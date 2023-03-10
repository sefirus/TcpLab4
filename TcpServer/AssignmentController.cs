using Core;
using Core.Entities;
using Core.Enums;
using Core.Exceptions;
using Core.Extensions;
using Core.Utils;
using TcpServer.Infrastructure;
using TcpServer.Repositories;

namespace TcpServer;

public class AssignmentController: ControllerBase
{
    private AssignmentRepository? _assignmentRepository;
    private QuestionsRepository? _questionsRepository;

    private AssignmentRepository AssignmentRepository => 
        _assignmentRepository ??= new AssignmentRepository(AssignmentsFolderPath);
    
    private QuestionsRepository QuestionsRepository => 
        _questionsRepository ??= new QuestionsRepository(QuestionsTemplatePath);

    [HandlerMethod(Commands.StartNewAssignment)]
    public Message Start(Message request)
    {
        var assignee = request.Parameters[Args.AssigneeName];
        var questions = QuestionsRepository.GetAll().Shuffle().Take(3);
        var newAssignment = new Assignment()
        {
            Id = Guid.NewGuid(),
            StartDate = DateTimeOffset.UtcNow,
            AssigneeName = assignee,
            Questions = questions.ToHashSet()
        };
        AssignmentRepository.CreateAssignment(newAssignment);
        var response = new Message()
        {
            Parameters = new Dictionary<string, string>
            {
                { "id", newAssignment.Id.ToString() }
            },
            Body = newAssignment
        };
        return response;
    }

    [HandlerMethod(Commands.GetAssignments)]
    public Message Get(Message request)
    {
        Message response;
        if (!request.Parameters.TryGetValue(Args.Secret, out var s) || s != Configuration["AccessSecret"])
        {
            throw new IncorrectSecretException();
        }
        
        if (request.Parameters.TryGetValue(Args.AssignmentId, out var id))
        {
            var assignment = AssignmentRepository.GetById(id);
            if (assignment is null)
            {
                return Message.GetResponseError("Not found!");
            }
            response = new Message()
            {
                Parameters = new Dictionary<string, string>
                {
                    { "id", assignment.Id.ToString() },
                    { "count", "1" }
                },
                Body = assignment
            };       
        }
        else
        {
            var assignments = AssignmentRepository.GetAll();
            response = new Message()
            {
                Parameters = new Dictionary<string, string>
                {
                    { "count", $"{assignments.Count()}" }
                },
                Body = assignments
            };  
        }

        return response;
    }

    [HandlerMethod(Commands.AnswerQuestion)]
    public Message Answer(Message request)
    {
        if (!request.Parameters.ContainsKey(Args.AssignmentId))
        {
            return Message.GetResponseError("Request does not contain AssignmentId!");
        }

        var assignment = AssignmentRepository.GetById(request.Parameters[Args.AssignmentId]);
        if (assignment is null || assignment.EndDate != default)
        {
            return Message.GetResponseError("No active assignment with such Id!");
        }

        if (!request.Parameters.TryGetValue(Args.QuestionId, out var questionId)
            || !Guid.TryParse(questionId, out var questionGuid)
            || !assignment.Questions.Any(q => q.Id == questionGuid))
        {
            return Message.GetResponseError("Bad request!");
        }

        var question = assignment.Questions.First(q => q.Id == questionGuid);
        if (!request.Parameters.TryGetValue(Args.OptionId, out var optionId)
            || !Guid.TryParse(optionId, out var optionGuid)
            || !question.Options.Any(q => q.Id == optionGuid))
        {
            return Message.GetResponseError("Bad request!");
        }

        question.ChosenAnswerId = optionGuid;
        question.ChosenAnswer = question.Options.FirstOrDefault(q => q.Id == optionGuid);
        if (assignment.Questions.All(q => q.ChosenAnswerId != default))
        {
            assignment.EndDate = DateTimeOffset.UtcNow;
        }
        AssignmentRepository.Update(assignment);
        var response = new Message()
        {
            Parameters = new Dictionary<string, string>
            {
                { "id", assignment.Id.ToString() }
            },
            Body = assignment
        };
        return response;
    }
    
}