using EventManager.Application.Services.Interfaces;
using EventManager.Domain.Models;
using System.Collections.Concurrent;

namespace EventManager.Application.Services;

public class InMemoryRepository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
{
    private readonly ConcurrentDictionary<Guid, TEntity> _repository;

    public InMemoryRepository(Dictionary<Guid, TEntity>? repository = null)
    {
        _repository = repository == null
            ? new ConcurrentDictionary<Guid, TEntity>()
            : new ConcurrentDictionary<Guid, TEntity>(repository);
    }

    public Task<TEntity?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult(_repository.GetValueOrDefault(id));

    public Task<IEnumerable<TEntity>> GetListAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IEnumerable<TEntity>>(_repository.Values.ToList());

    public Task CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _repository[entity.Id] = entity;
        return Task.CompletedTask;
    }

    public Task<bool> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        while (_repository.TryGetValue(entity.Id, out var existing))
        {
            if (_repository.TryUpdate(entity.Id, entity, existing))
                return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult(_repository.TryRemove(id, out _));
}
