using EventManager.Domain.Models;

namespace EventManager.Application.Services.Interfaces;

public interface IRepository<TEntity> where TEntity : BaseEntity
{
    Task<TEntity?> Get(Guid id, CancellationToken cancellationToken = default);
    Task<List<TEntity>> GetList(CancellationToken cancellationToken = default);
    Task Create(TEntity entity, CancellationToken cancellationToken = default);
    Task<bool> Update(TEntity entity, CancellationToken cancellationToken = default);
    Task<bool> Delete(Guid id, CancellationToken cancellationToken = default);
}
