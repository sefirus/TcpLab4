using Core.Exceptions;

namespace Core.Helpers;

public class Args
{
    public const string AssigneeName = "AssigneeName";
    public const string AssignmentId = "AssignmentId";
    public const string QuestionIndex = "QuestionIndex";
    public const string QuestionId = "QuestionId";
    public const string OptionIndex = "OptionIndex";
    public const string OptionId = "OptionId";
    public static Dictionary<string, string> Parse(string argString)
    {
        var aliasMap = new Dictionary<string, string>
        {
            { "-n", AssigneeName },
            { "--Name", AssigneeName },
            { "-a", AssignmentId },
            { "--AssignmentId", AssignmentId },
            { "-q", QuestionIndex },
            { "--QuestionIndex", QuestionIndex },
            { "-qid", QuestionId },
            { "--QuestionId", QuestionId },
            { "-o", OptionIndex },
            { "--OptionIndex", OptionIndex },
            { "-oid", OptionId },
            { "--OptionId", OptionId },
        };

        var result = new Dictionary<string, string>();

        var args = argString.Split(' ');

        for (var i = 0; i < args.Length; i += 2)
        {
            if (i + 1 >= args.Length)
            {
                // If there is an odd number of arguments, the last one is ignored
                break;
            }

            var alias = args[i];
            var value = args[i + 1];

            if (aliasMap.TryGetValue(alias, out string name))
            {
                result[name] = value;
            }
            else
            {
                throw new BadCommandException("Argument has no alias!", alias);
            }
        }

        return result;
    }
}