using Core.Exceptions;
using Core.Interfaces.Infrastructure;
using Core.Utils;

namespace TcpClient.Infrastructure;

public class CliApp : IApplication
{
    public Dictionary<string, (string, CommandHandlerBase,
        Func<CommandHandlerBase, Dictionary<string, string>, string>)> Commands { get; init; }
    public void Run()
    {
        Print.Line();
        Print.Help();
        while (true)
        {
            Print.Line();
            var input = Console.ReadLine();
            var command = input?.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            if (Commands.TryGetValue(command ?? string.Empty, out var handler))
            {
                try
                {
                    var argsString = input.Remove(0, command.Length);
                    var args = Args.Parse(argsString!);
                    var messageToWrite = handler.Item3(handler.Item2, args);
                    Console.WriteLine(messageToWrite);
                }
                catch (BadCommandException badCommandException)
                {
                    Console.WriteLine("Bad command! Check yor input!");
                    Console.WriteLine($"\tArg: {badCommandException.Arg}");
                    if (badCommandException.ArgValue is not null)
                    {
                        Console.WriteLine($"\tValue: {badCommandException.ArgValue}");
                    }

                    Console.WriteLine($"\tArg: {badCommandException.Message}");
                }
            }
            else
            {
                Console.WriteLine("Wrong command!");
            }
        }
    }
}