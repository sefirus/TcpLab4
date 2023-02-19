using System.Diagnostics.CodeAnalysis;
using Core.Helpers;
using Core.Interfaces;

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

    public List<TEntity> GetEntities()
    {
        var entities = JsonHelper.ReadObject<List<TEntity>>(_filePath);
        _entities = entities ?? throw new ArgumentNullException($"FilePath", 
            "File you provided is not in correct format!");
        return _entities;       
    }

    public IQueryable<TEntity> GetEntitiesQueryable()
    {
        return GetEntities().AsQueryable();
    }
}