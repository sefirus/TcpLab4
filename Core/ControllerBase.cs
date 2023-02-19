namespace Core;

public class ControllerBase
{
    public string? QuestionsTemplatePath { get; init; }
    public string? AssignmentsFolderPath { get; init; }

    private Dictionary<string, string>? _configuration;
    public Dictionary<string, string> Configuration 
    { 
        get => _configuration;
        set => _configuration ??= value;
    }
}