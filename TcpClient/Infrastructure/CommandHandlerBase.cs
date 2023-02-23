using System.Net;
using System.Net.Sockets;
using System.Text;
using Core;
using Newtonsoft.Json;

namespace TcpClient.Infrastructure;

public class CommandHandlerBase
{
    public int Port { get; set; }
    protected Message SendMessage(Message request)
    {
        Console.OutputEncoding = Encoding.Unicode;
        Console.InputEncoding = Encoding.Unicode;
        var bytes = new byte[8192000];

        var ipHost = Dns.GetHostEntry("localhost");
        var ipAddr = ipHost.AddressList[0];
        var ipEndPoint = new IPEndPoint(ipAddr, Port);
        var sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        sender.Connect(ipEndPoint);
        var message = JsonConvert.SerializeObject(request);
        var msg = Encoding.UTF8.GetBytes(message);
        sender.Send(msg);
        var bytesRec = sender.Receive(bytes);

        sender.Shutdown(SocketShutdown.Both);
        sender.Close();
        return Message.Deserialize(bytes, bytesRec, out _);
    }
}