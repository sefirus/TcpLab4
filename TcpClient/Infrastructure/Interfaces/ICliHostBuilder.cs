using Core.Interfaces.Infrastructure;

namespace TcpClient.Infrastructure.Interfaces;

public interface ICliHostBuilder : IApplicationBuilder<CliHostBuilder, CommandHandlerBase, Dictionary<string, string>, string>
{
    ICliHostBuilder SetConfigurationPath(string filePath);
}