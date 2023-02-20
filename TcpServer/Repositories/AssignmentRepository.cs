using Core.Entities;
using Core.Helpers;
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
        var json = JsonConvert.SerializeObject(newAssignment, Formatting.Indented);
        var path = $@"{_folderPath}\Assignment_{DateTime.UtcNow.Ticks}_{newAssignment.Id}.json";
        File.WriteAllText(path, json);
    }

    public Assignment? GetById(string id)
    {
        var filePath = Directory
            .GetFiles(_folderPath)
            .FirstOrDefault(f => f.Contains(id));

        var result = JsonHelper.ReadObject<Assignment>(filePath);
        return result;
    }
    
    public IEnumerable<Assignment> GetAll()
    {
        var filePaths = Directory.GetFiles(_folderPath);
        foreach (var filePath in filePaths)
        {
            var result = JsonHelper.ReadObject<Assignment>(filePath);
            if (result is null)
            {
                continue;
            }
            yield return result;   
        }
    }
}