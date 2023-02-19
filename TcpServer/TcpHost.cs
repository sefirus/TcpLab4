using System.Net;
using System.Net.Sockets;
using System.Text;
using Core;
using Core.Helpers;
using Newtonsoft.Json;

namespace TcpServer;

public class TcpHost
{
    public int Port { get; private set; }

    private readonly Dictionary<string, (string, ControllerBase, Func<ControllerBase, Message, Message>)> _endpoints =
        new();

    private readonly Dictionary<string, string> _configuration;
    private string _questionsFilePath;
    private string _assignmentsFolderPath;

    public TcpHost(string settingsFilePath)
    {
        _configuration = JsonHelper.ReadObject<Dictionary<string, string>>(settingsFilePath)
            ?? throw new Exception("Cant read settings!");
    }

    public TcpHost AddQuestions(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentNullException(nameof(filePath), "Path you provided cant by null!");
        }

        _questionsFilePath = filePath;
        foreach (var keyValue in _endpoints)
        {
            //keyValue.Value.Item2.QuestionsTemplatePath ??= _questionsFilePath;
        }

        return this;
    }

    public TcpHost AddAssignmentsFolder(string folderPath)
    {
        if (string.IsNullOrWhiteSpace(folderPath))
        {
            throw new ArgumentNullException(nameof(folderPath), "Path you provided cant by null!");
        }

        _assignmentsFolderPath = folderPath;
        foreach (var keyValue in _endpoints)
        {
            //keyValue.Value.Item2.AssignmentsFolderPath ??= _assignmentsFolderPath;
        }

        return this;
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

    public TcpHost AddController<TController>() where TController : ControllerBase, new()
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
        var newController = new TController
        {
            AssignmentsFolderPath = _assignmentsFolderPath,
            QuestionsTemplatePath = _questionsFilePath,
            Configuration = _configuration
        };
        foreach (var methodInfo in controllerMethods)
        {
            var parameters = methodInfo.GetParameters();
            if (methodInfo.ReturnType != typeof(Message)
                || parameters.FirstOrDefault()?.ParameterType != typeof(Message)
                || parameters.Length != 1)
            {
                continue;
            }
            
            var genericFunc = (Func<TController, Message, Message>)Delegate
                .CreateDelegate(typeof(Func<TController, Message, Message>), null, methodInfo);
            var func = new Func<ControllerBase, Message, Message>((a, b) => genericFunc((TController)a, b));
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
        if (!_endpoints.Any() || _questionsFilePath is null || _assignmentsFolderPath is null)
        {
            throw new InvalidOperationException("Initialize data first!");
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
            while (true) HandleRequest(ipEndPoint, sListener);

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        return this;
    }

    private void HandleRequest(IPEndPoint ipEndPoint, Socket sListener)
    {
        Console.WriteLine($"Waiting for the connection on {ipEndPoint}");
        using var socketHandler = sListener.Accept();
        var bytes = new byte[1024];
        var bytesRec = socketHandler.Receive(bytes);
        var receivedMessage = Message.Deserialize(bytes, bytesRec, out var data);
        var response = Message.GetResponseError("Bad request");
        if (receivedMessage is not null
            && _endpoints.TryGetValue(receivedMessage.Address, out var requestHandler))
        {
            try
            {
                response = requestHandler.Item3(requestHandler.Item2, receivedMessage);
            } 
            catch (Exception e)
            {
                response.Body = e;
            }
        }

        var serializedResponse = JsonConvert.SerializeObject(response, Formatting.Indented);
        Console.WriteLine($"\tGenerated response: {serializedResponse}");
        socketHandler.Send(Encoding.UTF8.GetBytes(serializedResponse));
        if (data.Contains("<TheEnd>"))
        {
            Console.WriteLine("Closed connection");
            throw new Exception("Closed connection");
        }

        socketHandler.Shutdown(SocketShutdown.Both);
        socketHandler.Close();
    }
}