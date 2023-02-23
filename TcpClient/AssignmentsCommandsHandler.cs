using Core;
using Core.Entities;
using Core.Enums;
using Core.Exceptions;
using Core.Utils;
using TcpClient.Infrastructure;

namespace TcpClient;

public class AssignmentsCommandsHandler : CommandHandlerBase
{
    private Assignment? _currentAssignment;
    
    [HandlerMethod(Commands.StartNewAssignment)]
    public string StartAssignment(Dictionary<string, string> args)
    {
        args.EnsureAllKeys(Args.AssigneeName);
        var request = new Message()
        {
            Address = Commands.StartNewAssignment,
            Parameters = new Dictionary<string, string>()
            {
                { Args.AssigneeName, args[Args.AssigneeName] }
            }
        };
        var responseMessage = SendMessage(request);
        var responseBody = responseMessage.GetDeserializedBody<Assignment>();
        _currentAssignment = responseBody;
        return Print.Assignment(responseBody);
    }

    private Question GetQuestion(Dictionary<string, string> args)
    {
        if (args.TryGetValue(Args.QuestionId, out var id)
            && Guid.TryParse(id, out var guid))
        {
            var question = _currentAssignment?
                .Questions.FirstOrDefault(q => q.Id == guid);
            if (question is null)
            {
                throw new BadCommandException("Question does not exist!", Args.QuestionId, id);
            }

            return question;
        }

        if (args.TryGetValue(Args.QuestionIndex, out var indexString)
            && int.TryParse(indexString, out int index))
        {
            var question = _currentAssignment?
                .Questions.ElementAtOrDefault(index - 1);
            if (question is null)
            {
                throw new BadCommandException("Question does not exist!", Args.QuestionIndex, indexString);
            }

            return question;
        }

        throw new BadCommandException("Bad value for argument!", args: new []{Args.QuestionId, Args.QuestionIndex});
}
    private Option GetOption(Guid questionId, Dictionary<string, string> args)
    {
        var question = _currentAssignment?
            .Questions.FirstOrDefault(q => q.Id == questionId);
        if (args.TryGetValue(Args.OptionId, out var id)
            && Guid.TryParse(id, out var guid))
        {
            var option = question?.Options
                .FirstOrDefault(o => o.Id == guid);
            if (option is null)
            {
                throw new BadCommandException("Option does not exist!", Args.OptionId, id);
            }

            return option;
        }

        if (args.TryGetValue(Args.OptionIndex, out var indexString)
            && int.TryParse(indexString, out int index))
        {
            var option = question?.Options
                .ElementAtOrDefault(index - 1);
            if (option is null)
            {
                throw new BadCommandException("Option does not exist!", Args.OptionId, id);
            }

            return option;
        }

        throw new BadCommandException("Bad value for argument!", args: new []{Args.OptionId, Args.OptionIndex});
    }
    
    [HandlerMethod(Commands.AnswerQuestion)]
    public string Answer(Dictionary<string, string> args)
    {
        if (_currentAssignment is null)
        {
            throw new BadCommandException("Start the assignment first!");
        }
        args.EnsureOnlyOneKey(Args.QuestionId, Args.QuestionIndex);
        args.EnsureOnlyOneKey(Args.OptionId, Args.OptionIndex);
        var question = GetQuestion(args);
        var option = GetOption(question.Id, args);
        var request = new Message()
        {
            Address = Commands.AnswerQuestion,
            Parameters = new Dictionary<string, string>()
            {
                { Args.AssignmentId, _currentAssignment.Id.ToString() },
                { Args.QuestionId, question.Id.ToString() },
                { Args.OptionId, option.Id.ToString() }
            }
        };
        var responseMessage = SendMessage(request);
        var responseBody = responseMessage.GetDeserializedBody<Assignment>();
        _currentAssignment = responseBody;
        return Print.Assignment(responseBody);
    } 
}