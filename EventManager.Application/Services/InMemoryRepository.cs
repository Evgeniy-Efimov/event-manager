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

    public TEntity? Get(Guid id)
    {
        lock (_lock)
        {
            return _repository.GetValueOrDefault(id);
        }
    }

    public List<TEntity> GetList()
    {
        lock (_lock)
        {
            return _repository.Values.ToList();
        }
    }

    public void Create(TEntity entity)
    {
        lock (_lock)
        {
            _repository[entity.Id] = entity;
        }
    }

    public bool Update(TEntity entity)
    {
        lock (_lock)
        {
            if (!_repository.ContainsKey(entity.Id))
                return false;

            _repository[entity.Id] = entity;
            return true;
        }
    }

    public bool Delete(Guid id)
    {
        lock (_lock)
        {
            return _repository.Remove(id);
        }
    }
}
