using System.Net;
using System.Net.Sockets;
using System.Text;
using Core;
using Core.Helpers;
using Newtonsoft.Json;

namespace TcpClient;

public class ClientHandler
{
    private readonly bool _logToConsole;
    private int Port { get; set; }
    private readonly Dictionary<string, string> _configuration;
    private readonly Dictionary<string, Action<string>> _commands = new();

    private void StartAssignment(string args)
    {
        var request = new Message()
        {
            Address = "start",
            Parameters = new Dictionary<string, string>()
            {
                { "assigneeName", "sefirus" }
            }
        };
        
    }
    
    #region Infrastructure
    private Message SendMessage(Message request)
    {
        Console.OutputEncoding = Encoding.Unicode;
        Console.InputEncoding = Encoding.Unicode;
        var bytes = new byte[4096];

        var ipHost = Dns.GetHostEntry("localhost");
        var ipAddr = ipHost.AddressList[0];
        var ipEndPoint = new IPEndPoint(ipAddr, Port);
        var sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        sender.Connect(ipEndPoint);
        var message = JsonConvert.SerializeObject(request);
        var msg = Encoding.UTF8.GetBytes(message);
        sender.Send(msg);
        var bytesRec = sender.Receive(bytes);
        if (_logToConsole)
        {
            Console.WriteLine($"Answer: {Encoding.UTF8.GetString(bytes, 0, bytesRec)}");
        }
        sender.Shutdown(SocketShutdown.Both);
        sender.Close();
        return Message.Deserialize(bytes, bytesRec, out _);
    }
    public ClientHandler(string settingsFilePath, bool logToConsole = false)
    {
        _logToConsole = logToConsole;
        _configuration = JsonHelper.ReadObject<Dictionary<string, string>>(settingsFilePath) 
                         ?? throw new Exception("Cant read settings!");
        if (!int.TryParse(_configuration["Port"], out var port))
        {
            throw new ArgumentNullException($"Port", "Port you provided is not in correct format!");
        }

        Port = port;
    }

    public ClientHandler InitializeCommands()
    {
        _commands.Add("new", StartAssignment);
        _commands.Add("ans", StartAssignment);
        _commands.Add("ext", StartAssignment);
        _commands.Add("get", StartAssignment);
        return this;
    }

    public void Run()
    {
        while (true)
        {
            var input = Console.ReadLine();
            var command = input?.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            if (_commands.TryGetValue(command ?? string.Empty, out var handler))
            {
                handler(command!);
            }
            else
            {
                Console.WriteLine("Wrong command!");
            }
        }
    }
    #endregion

}