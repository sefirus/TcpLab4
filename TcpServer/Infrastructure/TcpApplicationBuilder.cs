using System.Net;
using System.Net.Sockets;
using System.Text;
using Core;
using Core.Infrastructure;
using Core.Interfaces.Infrastructure;
using TcpServer.Infrastructure.Interfaces;

namespace TcpServer.Infrastructure;

public class TcpApplicationBuilder : BuilderBase<ITcpHostBuilder, ControllerBase, Message, Message>, ITcpHostBuilder
{
    private string _questionsFilePath;
    private string _assignmentsFolderPath;
    public TcpApplicationBuilder()
    {
        Child = this;
    }

    public ITcpHostBuilder AddQuestions(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentNullException(nameof(filePath), "Path you provided cant by null!");
        }

        _questionsFilePath = filePath;
        foreach (var pair in Endpoints)
        {
            pair.Value.Item2.QuestionsTemplatePath = _questionsFilePath;
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
        foreach (var pair in Endpoints)
        {
            pair.Value.Item2.AssignmentsFolderPath = _assignmentsFolderPath;
        }
        return this;
    }
    
    public IApplication Build()
    {
        if (!Endpoints.Any() || _questionsFilePath is null || _assignmentsFolderPath is null)
        {
            throw new InvalidOperationException("Initialize data first!");
        }
        foreach (var pair in Endpoints)
        {
            pair.Value.Item2.Configuration = Configuration;
        }

        Console.OutputEncoding = Encoding.Unicode;
        Console.InputEncoding = Encoding.Unicode;

        var ipHost = Dns.GetHostEntry("localhost");
        var ipAddr = ipHost.AddressList[0];
        var ipEndPoint = new IPEndPoint(ipAddr, Port);
        var sListener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        var webApp = new TcpApp()
        {
            Endpoints = Endpoints,
            SocketListener = sListener,
            IpEndPoint = ipEndPoint
        };
        return webApp;
    }
}