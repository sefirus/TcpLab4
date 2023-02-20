using System.Net;
using System.Net.Sockets;
using System.Text;
using Core;
using Core.Helpers;
using Core.Interfaces;

namespace TcpServer.Infrastructure;

public class TcpHostBuilder : ITcpHostBuilder
{
    private int _port;
    private readonly Dictionary<string, (string, ControllerBase, Func<ControllerBase, Message, Message>)> _endpoints =
        new();

    private readonly Dictionary<string, string> _configuration;
    private string _questionsFilePath;
    private string _assignmentsFolderPath;
    public TcpHostBuilder(string settingsFilePath)
    {
        _configuration = JsonHelper.ReadObject<Dictionary<string, string>>(settingsFilePath)
                         ?? throw new Exception("Cant read settings!");
    }

    public ITcpHostBuilder AddQuestions(string filePath)
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

    public ITcpHostBuilder AddAssignmentsFolder(string folderPath)
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

    public ITcpHostBuilder InitializeHost()
    {
        if (!int.TryParse(_configuration["Port"], out var port))
        {
            throw new ArgumentNullException($"Port", "Port you provided is not in correct format!");
        }

        _port = port;
        return this;
    }

    public ITcpHostBuilder AddController<TController>() where TController : ControllerBase, new()
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

    public ITcpApp Build()
    {
        if (!_endpoints.Any() || _questionsFilePath is null || _assignmentsFolderPath is null)
        {
            throw new InvalidOperationException("Initialize data first!");
        }

        Console.OutputEncoding = Encoding.Unicode;
        Console.InputEncoding = Encoding.Unicode;

        var ipHost = Dns.GetHostEntry("localhost");
        var ipAddr = ipHost.AddressList[0];
        var ipEndPoint = new IPEndPoint(ipAddr, _port);
        var sListener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        var webApp = new TcpApp()
        {
            Endpoints = _endpoints,
            SocketListener = sListener,
            IpEndPoint = ipEndPoint
        };
        return webApp;
    }
}