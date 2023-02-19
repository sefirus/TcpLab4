using TcpServer;

var repoDirectory = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.Parent?.FullName;
var settingsPath = $@"{repoDirectory}\Core\Settings.json";
var questionsPath = $@"{repoDirectory}\Core\Questions.json";
var assignmentsFolderPath = $@"{repoDirectory}\TcpServer/Assignments";
new TcpHost(settingsPath)
    .AddControllers<AssignmentController>()
    .AddAssignmentsFolder(assignmentsFolderPath)
    .AddQuestions(questionsPath)
    .InitializeHost()
    .Run();