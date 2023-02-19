using Core.Entities;
using Newtonsoft.Json;

namespace TcpServer.Repositories;

public class AssignmentRepository
{
    private readonly string _folderPath;

    public AssignmentRepository(string folderPath)
    {
        _folderPath = folderPath;
    }

    public void CreateAssignment(Assignment newAssignment)
    {
        var json = JsonConvert.SerializeObject(newAssignment);
        File.WriteAllText($@"{_folderPath}\Assignment_{newAssignment.Id}.json", json);
    }
}