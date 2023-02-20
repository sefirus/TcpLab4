using TcpServer;
using TcpServer.Infrastructure;

var repoDirectory = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.Parent?.FullName;
var settingsPath = $@"{repoDirectory}\Core\Settings.json";
var questionsPath = $@"{repoDirectory}\Core\Questions.json";
var assignmentsFolderPath = $@"{repoDirectory}\TcpServer\Assignments";

var webApp = new TcpHostBuilder(settingsPath)
    .InitializeHost()
    .AddAssignmentsFolder(assignmentsFolderPath)
    .AddQuestions(questionsPath)
    .AddController<AssignmentController>()
    .Build();

webApp.Run();    