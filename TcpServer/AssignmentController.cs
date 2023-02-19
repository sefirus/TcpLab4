using Core;
using Core.Entities;
using Core.Extensions;
using TcpServer.Repositories;
using TcpServer.Utilities;

namespace TcpServer;

public class AssignmentController: ControllerBase
{
    private List<Question> _questions = new();
    private AssignmentRepository _assignmentRepository;

    public AssignmentController()
    {
        _assignmentRepository = new AssignmentRepository(AssignmentsFolderPath);
    }

    private List<Question> GetQuestions()
    {
        if (!_questions.Any())
        {
            _questions = new ListGetter<Question>()
                .SetFilePath(QuestionsTemplatePath)
                .ReadEntities()
                .GetEntities();
        }

        return _questions;
    }
    
    [ControllerMethod("start")]
    public Message Start(Message request)
    {
        var assignee = request.Parameters["assigneeName"];
        var questions = GetQuestions().Shuffle().Take(3);
        var newAssignment = new Assignment()
        {
            Id = Guid.NewGuid(),
            StartDate = DateTimeOffset.UtcNow,
            AssigneeName = assignee,
            Questions = questions.ToHashSet()
        };
        _assignmentRepository.CreateAssignment(newAssignment);
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
}