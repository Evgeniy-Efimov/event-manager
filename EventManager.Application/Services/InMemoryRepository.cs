using EventManager.Application.Services.Interfaces;
using EventManager.Domain.Models;

namespace EventManager.Application.Services;

public class InMemoryRepository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
{
    private readonly Dictionary<Guid, TEntity> _repository;
    private static readonly Lock _lock = new();

    public InMemoryRepository(Dictionary<Guid, TEntity>? repository = null)
    {
        _repository = repository ?? [];
    }

    public Task<TEntity?> Get(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            return Task.FromResult(_repository.GetValueOrDefault(id));
        }
    }

    public Task<IEnumerable<TEntity>> GetList(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            return Task.FromResult(_repository.Values.AsEnumerable());
        }
    }

    public Task Create(TEntity entity, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _repository[entity.Id] = entity;
            return Task.CompletedTask;
        }
    }

    public Task<bool> Update(TEntity entity, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (!_repository.ContainsKey(entity.Id))
                return Task.FromResult(false);

            _repository[entity.Id] = entity;
            return Task.FromResult(true);
        }
    }

    public Task<bool> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            return Task.FromResult(_repository.Remove(id));
        }
    }
}
