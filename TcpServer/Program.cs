using TcpServer;

var projectDirectory = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.Parent?.FullName;
var filePath = $@"{projectDirectory}\Core\Settings.json";
new TcpHost(filePath)
    .InitializeHost()
    .AddControllers<AssignmentController>()
    .Run();