using System.Net;
using System.Net.Sockets;
using System.Text;
using Core;
using Core.Interfaces;
using Newtonsoft.Json;

namespace TcpServer.Infrastructure;

public class TcpApp : ITcpApp
{
    private readonly Dictionary<string, (string, ControllerBase, Func<ControllerBase, Message, Message>)> _endpoints;
    public int Port { get; private set; }
    public IPEndPoint IpEndPoint { get; init; }
    public Socket SocketListener { get; init; }

    public void Run()
    {
        try
        {
            SocketListener.Bind(IpEndPoint);
            SocketListener.Listen(10);
            while (true) HandleRequest(IpEndPoint, SocketListener);

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
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