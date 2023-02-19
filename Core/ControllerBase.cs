namespace Core;

public class ControllerBase
{
    private string? _questionsTemplatePath;
    public string? QuestionsTemplatePath
    {
        get => _questionsTemplatePath;
        set => _questionsTemplatePath ??= value;
    }

    private string? _assignmentsFolderPath;
    public string? AssignmentsFolderPath
    {
        get => _assignmentsFolderPath;
        set => _assignmentsFolderPath ??= value;
    }

    private Dictionary<string, string>? _configuration;
    public Dictionary<string, string> Configuration 
    { 
        get => _configuration;
        set => _configuration ??= value;
    }
}