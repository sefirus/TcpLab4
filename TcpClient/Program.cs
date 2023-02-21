using TcpClient;

var repoDirectory = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.Parent?.FullName;
var settingsPath = $@"{repoDirectory}\Core\Settings.json";
new ClientHandler(settingsPath)
    .InitializeCommands()
    .Run();