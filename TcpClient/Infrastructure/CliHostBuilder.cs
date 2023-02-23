using Core.Helpers;
using Core.Infrastructure;
using Core.Interfaces.Infrastructure;
using TcpClient.Infrastructure.Interfaces;

namespace TcpClient.Infrastructure;

public class CliHostBuilder : BuilderBase<CliHostBuilder, CommandHandlerBase, Dictionary<string, string>, string>, ICliHostBuilder
{
    private int Port { get; set; }
    private Dictionary<string, string> _configuration;

    public ICliHostBuilder SetConfigurationPath(string filePath)
    {
        _configuration = JsonHelper.ReadObject<Dictionary<string, string>>(filePath)
                         ?? throw new Exception("Cant read settings!");
        if (!int.TryParse(_configuration["Port"], out var port))
        {
            throw new ArgumentNullException($"Port", "Port you provided is not in correct format!");
        }

        Port = port;
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