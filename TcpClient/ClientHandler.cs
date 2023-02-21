using Core;
using Core.Entities;
using Core.Exceptions;
using TcpClient.Utils;

namespace TcpClient;

public partial class ClientHandler
{
    private Assignment? _currentAssignment;
    private void StartAssignment(Dictionary<string, string> args)
    {
        args.EnsureAllKeys(Args.AssigneeName);
        var request = new Message()
        {
            Address = "start",
            Parameters = new Dictionary<string, string>()
            {
                { "assigneeName", args[Args.AssigneeName] }
            }
        };
        var responseMessage = SendMessage(request);
        var responseBody = responseMessage.GetDeserializedBody<Assignment>();
        _currentAssignment = responseBody;
        Print.Assignment(responseBody);
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
                .Questions.ElementAtOrDefault(index);
            if (question is null)
            {
                throw new BadCommandException("Question does not exist!", Args.QuestionIndex, indexString);
            }

            return question;
        }

        throw new BadCommandException("Bad value for argument!", args: new []{Args.QuestionId, Args.QuestionIndex});
}
    
    public void Answer(Dictionary<string, string> args)
    {
        args.EnsureOnlyOneKey(Args.QuestionId, Args.QuestionIndex);
        args.EnsureOnlyOneKey(Args.OptionId, Args.OptionIndex);
        var questionId = GetQuestion(args).Id;
    }
}