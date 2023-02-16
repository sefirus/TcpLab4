using System.Net;
using System.Net.Sockets;
using System.Text;

Console.OutputEncoding = Encoding.Unicode;
Console.InputEncoding = Encoding.Unicode;

var ipHost = Dns.GetHostEntry("localhost");
var ipAddr = ipHost.AddressList[0];
var ipEndPoint = new IPEndPoint(ipAddr, 54739);

using var sListener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
try
{
    sListener.Bind(ipEndPoint);
    sListener.Listen(10);
    while (true)
    {
        Console.WriteLine($"Waiting for the connection on {ipEndPoint}");

        using var handler = sListener.Accept();
        string? data = null;
        var bytes = new byte[1024];
        var bytesRec = handler.Receive(bytes);
        data += Encoding.UTF8.GetString(bytes, 0, bytesRec);
        Console.Write($"Received data {data}");
        var reply = $"Tnx fot request of {data.Length} bytes";
        var msg = Encoding.UTF8.GetBytes(reply);
        handler.Send(msg);
        if (data.Contains("<TheEnd>"))
        {
            Console.WriteLine("Closed connection");
            break;
        }

        handler.Shutdown(SocketShutdown.Both);
        handler.Close();
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
}
finally
{
    Console.ReadLine();
}