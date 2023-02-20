namespace Core.Exceptions;

public class BadCommandException : Exception
{
    public BadCommandException(string? message, string arg, string? argValue = null) : base(message)
    {
        Arg = arg;
        ArgValue = argValue;
    }

    public string Arg { get; }
    public string? ArgValue { get; }
}