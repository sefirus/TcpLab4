namespace Core.Exceptions;

public class BadCommandException : Exception
{
    public BadCommandException(string? message, string arg, string? argValue = null) : base(message)
    {
        Arg = arg;
        ArgValue = argValue;
    }

    public BadCommandException(string? message, params string[] args) : base(message)
    {
        Arg = string.Join(", ", args);;
    }
    
    public string Arg { get; }
    public string? ArgValue { get; }
}