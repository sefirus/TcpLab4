using Core.Interfaces.Infrastructure;

namespace TcpClient.Infrastructure.Interfaces;

public interface ICliHostBuilder : IApplicationBuilder<ICliHostBuilder, CommandHandlerBase, Dictionary<string, string>, string>
{
}