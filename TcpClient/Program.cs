using TcpClient;
using TcpClient.Infrastructure;

var repoDirectory = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.Parent?.FullName;
var settingsPath = $@"{repoDirectory}\Core\Settings.json";

var cliApp = new CliHostBuilder()
    .AddHandler<AssignmentsCommandsHandler>()
    .AddConfiguration(settingsPath)
    .Build();

cliApp.Run();  