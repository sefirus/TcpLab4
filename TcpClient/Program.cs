using System.Net;
using System.Net.Sockets;
using System.Text;

try
{
    SendMessageFromSocket(54739);

}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
}
finally
{
    Console.ReadLine();
}


void SendMessageFromSocket(int port)
{
    Console.OutputEncoding = Encoding.Unicode;
    Console.InputEncoding = Encoding.Unicode;
    var bytes = new byte[1024];

    var ipHost = Dns.GetHostEntry("localhost");
    var ipAddr = ipHost.AddressList[0];
    var ipEndPoint = new IPEndPoint(ipAddr, port);
    var sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
    sender.Connect(ipEndPoint);
    Console.Write("Enter a message: ");
    var message = Console.ReadLine();
    Console.WriteLine($"Connected to the {sender.RemoteEndPoint}");
    var msg = Encoding.UTF8.GetBytes(message);
    sender.Send(msg);
    var bytesRec = sender.Receive(bytes);
    Console.WriteLine($"Answer: {Encoding.UTF8.GetString(bytes, 0, bytesRec)}");
    if (message.Contains("<TheEnd>"))
    {
        SendMessageFromSocket(port);
    }
    sender.Shutdown(SocketShutdown.Both);
    sender.Close();
}
