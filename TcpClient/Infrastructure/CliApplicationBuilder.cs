using Core.Infrastructure;
using Core.Interfaces.Infrastructure;
using TcpClient.Infrastructure.Interfaces;

namespace TcpClient.Infrastructure;

public class CliApplicationBuilder : BuilderBase<ICliHostBuilder, CommandHandlerBase, Dictionary<string, string>, string>, ICliHostBuilder
{
    public CliApplicationBuilder()
    {
        Child = this;
    }
    
    public new ICliHostBuilder AddConfiguration(string filePath)
    {
        base.AddConfiguration(filePath);
        
        foreach (var pair in Endpoints)
        {
            pair.Value.Item2.Port = Port;
        }

        return this;
    }

    public IApplication Build()
    {
        return new CliApp()
        {
            Commands = Endpoints
        };
    }
}