using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Core;

public class Message
{
    public static Message GetResponseError(string message)
    {
        var errorMessage = new Message()
        {
            Body = message
        };
        return errorMessage;
    }

    public TBody? GetDeserializedBody<TBody>()
    {
        return Body switch
        {
            JObject body => body.ToObject<TBody>(),
            JArray bodyArray => bodyArray.ToObject<TBody>(),
            _ => default
        };
    }

    public static Message? Deserialize(byte[] bytes, int bytesRec, out string received, bool logToConsole = false)
    {
        received = Encoding.UTF8.GetString(bytes, 0, bytesRec);
        if(logToConsole)
        {
            Console.WriteLine($"\tReceived message: {received}");
        }   
        var receivedMessage = JsonConvert.DeserializeObject<Message>(received);
        return receivedMessage;
    }
    public string Address { get; set; }
    public Dictionary<string, string> Parameters { get; set; } = new();
    public object Body { get; set; } = new();
}