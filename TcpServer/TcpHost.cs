using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Core;
using Newtonsoft.Json;

namespace TcpServer;

public class TcpHost
{
    public int Port { get; private set; }
    private readonly Dictionary<string, (string, ControllerBase, Func<ControllerBase, Message, Message>)> _endpoints = new();
    private readonly Dictionary<string, string> _configuration;

    public TcpHost(string filePath)
    {
        try
        {
            var readText = File.ReadAllText(filePath);
            if (readText is null)
            {
                throw new InvalidOperationException("Read the file before accessing the trucks!");
            }
            _configuration = JsonConvert.DeserializeObject<Dictionary<string, string>>(readText) 
                ?? throw new ArgumentNullException($"FilePath", "File you provided is not in correct format!");
        }
        catch (Exception e)
        {
            throw new InvalidOperationException("File you provided can`t be accessed!", e);
        }
    }

    public TcpHost InitializeHost()
    {
        if (!int.TryParse(_configuration["Port"], out var port))
        {
            throw new ArgumentNullException($"Port", "Port you provided is not in correct format!");
        }
        Port = port;
        return this;
    }

    public TcpHost AddControllers<TController>() where TController: ControllerBase, new ()
    {
        var controllerType = typeof(TController);
        var attributeType = typeof(ControllerMethodAttribute);
        var isControllerAdded = _endpoints
            .Any(c => c.Value.Item1 == controllerType.ToString());
        if (isControllerAdded)
        {
            throw new InvalidOperationException($"Controller {controllerType} is already added!");
        }
        var controllerMethods = controllerType
            .GetMethods()
            .Where(m => m.GetCustomAttributes(attributeType, false).Length > 0);
        TController newController = new TController();
        foreach (var methodInfo in controllerMethods)
        {
            var parameters = methodInfo.GetParameters();
            if (methodInfo.ReturnType != typeof(Message) 
                || parameters.FirstOrDefault()?.ParameterType != typeof(Message)
                || parameters.Length != 1)
            {
                continue;
            }

            var func = (Func<TController, Message, Message>)Delegate
                    .CreateDelegate(typeof(Func<TController, Message, Message>), null, methodInfo)
                as Func<ControllerBase, Message, Message>;
            var address = methodInfo.CustomAttributes
                .FirstOrDefault(a => a.AttributeType == attributeType)?
                .ConstructorArguments?
                .FirstOrDefault()
                .Value?.ToString() ?? throw new InvalidOperationException("Controller contains invalid attributes!");
            _endpoints.Add(address, (controllerType.ToString(), newController, func));
            }
        return this;
    }

    public TcpHost Run()
    {
        if (_endpoints is null)
        {
            throw new InvalidOperationException("Initialize endpoints first!"); 
        }
        Console.OutputEncoding = Encoding.Unicode;
        Console.InputEncoding = Encoding.Unicode;

        var ipHost = Dns.GetHostEntry("localhost");
        var ipAddr = ipHost.AddressList[0];
        var ipEndPoint = new IPEndPoint(ipAddr, Port);

        using var sListener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            sListener.Bind(ipEndPoint);
            sListener.Listen(10);
            HandleRequest(ipEndPoint, sListener);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        return this;
    }

    private void HandleRequest(IPEndPoint ipEndPoint, Socket sListener)
    {
        while (true)
        {
            Console.WriteLine($"Waiting for the connection on {ipEndPoint}");
            using var socketHandler = sListener.Accept();
            string? data = null;
            var bytes = new byte[1024];
            var bytesRec = socketHandler.Receive(bytes);
            data += Encoding.UTF8.GetString(bytes, 0, bytesRec);
            Console.WriteLine($"\tReceived message: {data}");
            var receivedMessage = JsonConvert.DeserializeObject<Message>(data);
            var response = Message.GetResponseError("Bad request");
            if (receivedMessage is not null 
                && _endpoints.TryGetValue(receivedMessage.Address, out var requestHandler))
            {
                response = requestHandler.Item3(requestHandler.Item2, receivedMessage);
            }
            
            Console.WriteLine($"\tGenerated response: {response}");
            socketHandler.Send(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(response)));
            if (data.Contains("<TheEnd>"))
            {
                Console.WriteLine("Closed connection");
                break;
            }

            socketHandler.Shutdown(SocketShutdown.Both);
            socketHandler.Close();
        }
    }
}