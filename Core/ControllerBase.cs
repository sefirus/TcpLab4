namespace Core;

public class ControllerBase
{
    public string? QuestionsTemplatePath { get; set; }
    public string? AssignmentsFolderPath { get; set; }

    private Dictionary<string, string>? _configuration;
    public Dictionary<string, string> Configuration 
    { 
        get => _configuration;
        set => _configuration ??= value;
    }
}