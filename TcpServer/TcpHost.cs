using System.Net;
using System.Net.Sockets;
using System.Text;
using Core;
using Newtonsoft.Json;

namespace TcpServer;

public class TcpHost
{
    public int Port { get; private set; }
    private Dictionary<string, Func<Message, Message>>? _endpoints;
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

    public void InitializeHost()
    {
        if (!int.TryParse(_configuration["Port"], out var port))
        {
            throw new ArgumentNullException($"Port", "Port you provided is not in correct format!");
        }
        Port = port;
        _endpoints = new Dictionary<string, Func<Message, Message>>()
        {
            { _configuration["StartAssignment"], HandleStart },
        };
    }

    public void Start()
    {
        
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
    }

    private Message HandleStart(Message request)
    {
        throw new NotImplementedException();
    }
}