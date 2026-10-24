using EventManager.Domain.Models;

namespace EventManager.Application.Services.Interfaces;

public interface IQueue<TEntity> where TEntity : BaseEntity
{
    ValueTask EnqueueAsync(TEntity entity, CancellationToken cancellationToken = default);
    IAsyncEnumerable<TEntity> ReadAllAsync(CancellationToken cancellationToken = default);
    ValueTask<TEntity?> ReadAsync(CancellationToken cancellationToken = default);
}
