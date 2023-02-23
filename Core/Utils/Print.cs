using Core.Entities;

namespace Core.Utils;

public class Print
{
    private static string PrintAssignment(Assignment assignment, int indentLevel = 0)
    {
        var isFinished = assignment.EndDate != default;
        var indent = new string(' ', indentLevel * 2);
        var result = $"{indent}AssigneeName: {assignment.AssigneeName}," + Environment.NewLine;
        result += $"{indent}StartDate: {assignment.StartDate}," + Environment.NewLine;
        result += $"{indent}Questions: [" + Environment.NewLine;
        foreach (var question in assignment.Questions)
        {
            result += $"{PrintQuestion(question, indentLevel + 2, isFinished)}";
        }
        result += $"{indent}    ]" + Environment.NewLine;
        if (isFinished)
        {
            var correctAnswers = assignment.Questions.Count(q => q.ChosenAnswerId == q.CorrectAnswerId);
            result += $"The score is {correctAnswers} / {assignment.Questions.Count}" + Environment.NewLine;
        }
        return result;
    }

    private static string PrintQuestion(Question question, int indentLevel = 0, bool isFinished = false)
    {
        var indent = new string(' ', indentLevel * 2);
        var result = $"{indent}  @ Id: {question.Id}," + Environment.NewLine;
        result += $"{indent}    Content: {question.Content}," + Environment.NewLine;
        result += $"{indent}    Options: [" + Environment.NewLine;
        foreach (var option in question.Options)
        {
            var idLevel = indentLevel + 2;
            var isChosen = question.ChosenAnswerId == option.Id;
            var isCorrect = isFinished && question.CorrectAnswerId == option.Id;
            result += $"{PrintOption(option, idLevel, isChosen, isCorrect)}";
        }
        result += $"{indent}    ]" + Environment.NewLine;
        return result;
    }

    private static string PrintOption(Option option, int indentLevel = 0, bool isChosen = false, bool isCorrect = false)
    {
        var indent = new string(' ', indentLevel * 2);
        var tab = isChosen ? "==>" : " # ";
        tab = isCorrect ? $"&{tab}" : $" {tab}";
        var result = $"{indent}{tab}Id: {option.Id}," + Environment.NewLine;
        result += $"{indent}    Content: {option.Content}" + Environment.NewLine;
        return result;
    }
    public static void Line()
    {
        Console.WriteLine(new string('=', 100));
    }

    public static void Help()
    {
    Console.WriteLine(@"Usage: AssignmentsCLI <command> [options]

        Commands:
          start    Starts a new assignment for the specified assignee.
          answer   Answers a question in the current assignment.
          finish   Finishes the current assignment.
          get      Retrieves the details of an assignment.

        Options:
          -n, --name <AssigneeName>     Assignee name
          -a, --assignmentId <AssignmentId>
                                       Assignment ID (GUID)
          -q, --questionIndex <QuestionIndex>
                                       Question index (integer from 1 to 4)
          -qid, --questionId <QuestionId>
                                       Question ID (GUID)
          -o, --optionIndex <OptionIndex>
                                       Option index (integer from 1 to 4)
          -oid, --optionId <OptionId>  Option ID (GUID)
          -s, --secret <Secret>        Secret token
          -h, --help                   Show this help message and exit

        Description:
          This command-line interface (CLI) tool allows you to manage assignments and their questions and options.
          Use the 'start' command to start a new assignment for the specified assignee. You must provide the assignee name.
          Use the 'answer' command to answer a question in the current assignment for the specified assignee. You must provide either the question index or ID, and the option index or ID.
          Use the 'finish' command to finish the current assignment.
          Use the 'get' command to retrieve the details of an assignment. You must provide a secret token, and optionally an Assignment ID.

          Note that Question and Option IDs are GUIDs and Question and Option indices are integers from 1 to 4.

        Examples:
          AssignmentsCLI start --Name ""John Doe""
            Starts a new assignment for assignee ""John Doe"".

          AssignmentsCLI answer -n ""John Doe"" -qid ""96d2667c-72a4-4b57-8c71-3717de96b09f"" -o 1
            Answers option 1 of question with ID ""96d2667c-72a4-4b57-8c71-3717de96b09f"" in the current assignment for assignee ""John Doe"".

          AssignmentsCLI finish 
            Finishes the current assignment for assignee ""John Doe"".

          AssignmentsCLI get -s ""my_secret_token""
            Retrieves the details of all oh the assignments, using the secret token ""my_secret_token"".

          AssignmentsCLI get -n ""John Doe"" -a ""82ec1bf7-e02a-4ee9-a6e9-d2b82c332dba"" -s ""my_secret_token""
            Retrieves the details of the assignment with ID ""82ec1bf7-e02a-4ee9-a6e9-d2b82c332dba"", using the secret token ""my_secret_token"".");
    }

    
    public static string Assignment(Assignment assignment)
    {
        var result = PrintAssignment(assignment);
        Console.WriteLine(result);
        return result;
    }
}