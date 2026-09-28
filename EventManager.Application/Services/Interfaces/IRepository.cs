using EventManager.Domain.Models;

namespace EventManager.Application.Services.Interfaces;

public interface IRepository<TEntity> where TEntity : BaseEntity
{
    Task<TEntity?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> GetListAsync(CancellationToken cancellationToken = default);
    Task CreateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
