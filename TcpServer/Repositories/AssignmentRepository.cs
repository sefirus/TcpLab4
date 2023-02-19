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

    public Assignment? GetById(string id)
    {
        var filePath = Directory
            .GetFiles(_folderPath)
            .FirstOrDefault(f => f.Contains(id));

        if (filePath is null || !File.Exists(filePath))
        {
            return null;
        }
        
        var readText = File.ReadAllText(filePath);
        var result = JsonConvert.DeserializeObject<Assignment>(readText);
        return result;
    }
    
    public IEnumerable<Assignment> GetAll()
    {
        var filePaths = Directory.GetFiles(_folderPath);
        foreach (var filePath in filePaths)
        {
            if (!File.Exists(filePath))
            {
                continue;
            }
        
            var readText = File.ReadAllText(filePath);
            var result = JsonConvert.DeserializeObject<Assignment>(readText);
            if (result is null)
            {
                continue;
            }
            yield return result;   
        }
    }
}