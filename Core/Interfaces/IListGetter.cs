﻿using System.Diagnostics.CodeAnalysis;

namespace Core.Interfaces;

public interface IListGetter<TEntity>
{
    IEnumerable<TEntity> Entities { get; }
    IListGetter<TEntity> SetFilePath([NotNull]string path);
    IListGetter<TEntity> ReadEntities();
    List<TEntity> GetEntities();
    IQueryable<TEntity> GetEntitiesQueryable();    
}