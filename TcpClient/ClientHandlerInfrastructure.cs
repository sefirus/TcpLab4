using System.Net;
using System.Net.Sockets;
using System.Text;
using Core;
using Core.Enums;
using Core.Exceptions;
using Core.Helpers;
using Core.Utils;
using Newtonsoft.Json;

namespace TcpClient;

public partial class ClientHandler
{
    private readonly bool _logToConsole;
    private int Port { get; set; }
    private readonly Dictionary<string, string> _configuration;
    private readonly Dictionary<string, Action<Dictionary<string, string>>> _commands = new();

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
        _commands.Add(RoutesEnum.StartNewAssignment, StartAssignment);
        _commands.Add(RoutesEnum.AnswerQuestion, Answer);
        _commands.Add(RoutesEnum.FinishAssignment, StartAssignment);
        _commands.Add(RoutesEnum.GetAssignments, StartAssignment);
        _commands.Add(RoutesEnum.Help, StartAssignment);
        return this;
    }

    public void Run()
    {
        Print.Line();
        Print.Help();
        while (true)
        {
            Print.Line();
            var input = Console.ReadLine();
            var command = input?.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            if (_commands.TryGetValue(command ?? string.Empty, out var handler))
            {
                try
                {
                    var argsString = input.Remove(0, command.Length);
                    var args = Args.Parse(argsString!);
                    handler(args);
                }
                catch (BadCommandException badCommandException)
                {
                    Console.WriteLine("Bad command! Check yor input!");
                    Console.WriteLine($"\tArg: {badCommandException.Arg}");
                    if (badCommandException.ArgValue is not null)
                    {
                        Console.WriteLine($"\tValue: {badCommandException.ArgValue}");
                    }

                    Console.WriteLine($"\tArg: {badCommandException.Message}");
                }
            }
            else
            {
                Console.WriteLine("Wrong command!");
            }
        }
    }
}