using System.Diagnostics.CodeAnalysis;
using Core.Interfaces;
using Newtonsoft.Json;

namespace TcpServer.Utilities;

public class ListGetter<TEntity> : IListGetter<TEntity>
{
    private string? _filePath;
    private List<TEntity> _entities = new();
    private string? _readFile;
    
    public IEnumerable<TEntity> Entities => _entities;
    
    public IListGetter<TEntity> SetFilePath([NotNull]string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentNullException(nameof(path), "Path you provided cant by null!");
        }
        _filePath = path;
        return this;   
    }

    public IListGetter<TEntity> ReadEntities()
    {
        if (string.IsNullOrWhiteSpace(_filePath))
        {
            throw new InvalidOperationException("Set file path before reading!");
        }

        try
        {
            _readFile = File.ReadAllText(_filePath);
        }
        catch (Exception e)
        {
            throw new InvalidOperationException("File you provided can`t be accessed!", e);
        }

        return this;
    }

    public List<TEntity> GetEntities()
    {
        if (_readFile is null)
        {
            throw new InvalidOperationException("Read the file before accessing the trucks!");
        }
        var trailers = JsonConvert.DeserializeObject<List<TEntity>>(_readFile);
        _entities = trailers ?? throw new ArgumentNullException($"FilePath", 
            "File you provided is not in correct format!");
        return _entities;       }

    public IQueryable<TEntity> GetEntitiesQueryable()
    {
        return GetEntities().AsQueryable();
    }
}