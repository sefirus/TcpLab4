using Core;
using Core.Interfaces.Infrastructure;

namespace TcpServer.Infrastructure.Interfaces;

public interface ITcpHostBuilder : IApplicationBuilder<ITcpHostBuilder, ControllerBase, Message, Message> 
{
    ITcpHostBuilder AddQuestions(string filePath);
    ITcpHostBuilder AddAssignmentsFolder(string folderPath);
    ITcpHostBuilder InitializeHost();
}
