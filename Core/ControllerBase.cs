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
}