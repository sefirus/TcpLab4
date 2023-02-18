namespace Core;

public class Message
{
    public static Message GetError(string message)
    {
        var errorMessage = new Message()
        {
            Body = message
        };
        return errorMessage;
    }
    public string Address { get; set; }
    public Dictionary<string, string> Parameters { get; set; } = new();
    public object Body { get; set; } = new();
}