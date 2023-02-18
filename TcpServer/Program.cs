using TcpServer;

var filePath = @"D:\Repositories\Univ\Ookp\TcpLab4\Core\Settings.json";
new TcpHost(filePath)
    .InitializeHost()
    .Run();