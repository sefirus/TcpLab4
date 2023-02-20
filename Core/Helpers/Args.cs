using Core.Exceptions;

namespace Core.Helpers;

public static class Args
{
    public const string AssigneeName = "AssigneeName";
    public const string AssignmentId = "AssignmentId";
    public const string QuestionIndex = "QuestionIndex";
    public const string QuestionId = "QuestionId";
    public const string OptionIndex = "OptionIndex";
    public const string OptionId = "OptionId";
    private static readonly Dictionary<string, string> AliasMap = new()
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
    
    public static Dictionary<string, string> Parse(string argString)
    {
        var result = new Dictionary<string, string>();

        var args = argString.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

        for (var i = 0; i < args.Length; i += 2)
        {
            if (i + 1 >= args.Length)
            {
                // If there is an odd number of arguments, the last one is ignored
                break;
            }

            var alias = args[i];
            var value = args[i + 1];

            if (AliasMap.TryGetValue(alias, out string name))
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

    public static void EnsureKeys(this Dictionary<string, string> dictionary, params string[] requiredKeysNames)
    {
        foreach (var param in requiredKeysNames)
        {
            if (!dictionary.TryGetValue(param, out _))
            {
                throw new BadCommandException($"{param} Is required argument!", param);
            }
        }
    }
    public static Dictionary<string, string> Parse(string argString, params string[] requiredArgsNames)
    {
        var parsed = Parse(argString);
        parsed.EnsureKeys(requiredArgsNames);
        return parsed;
    }

}