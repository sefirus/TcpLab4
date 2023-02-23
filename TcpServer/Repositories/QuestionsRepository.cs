using Core.Entities;
using Core.Helpers;

namespace TcpServer.Repositories;

public class QuestionsRepository
{
    private readonly string _filePath;

    public QuestionsRepository(string filePath)
    {
        _filePath = filePath;
    }
    public List<Question> GetAll()
    {
        return JsonHelper.ReadObject<List<Question>>(_filePath);
    }
}