namespace Core.Interfaces;

public interface ITcpHostBuilder
{
    ITcpHostBuilder AddQuestions(string filePath);
    ITcpHostBuilder AddAssignmentsFolder(string folderPath);
    ITcpHostBuilder InitializeHost();
    ITcpHostBuilder AddController<TController>() where TController : ControllerBase, new();
    ITcpApp Build();
}
