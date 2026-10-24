using EventManager.Application.Services.Interfaces;
using EventManager.Domain.Models;
using System.Threading.Channels;

namespace EventManager.Application.Services;

public class InMemoryQueue<TEntity>(Channel<TEntity> channel) : IQueue<TEntity> where TEntity : BaseEntity
{
    public ValueTask EnqueueAsync(TEntity entity, CancellationToken cancellationToken = default)
        => channel.Writer.WriteAsync(entity, cancellationToken);

    public IAsyncEnumerable<TEntity> ReadAllAsync(CancellationToken cancellationToken = default)
        => channel.Reader.ReadAllAsync(cancellationToken);

    public ValueTask<TEntity?> ReadAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return channel.Reader.TryRead(out var entity)
            ? ValueTask.FromResult<TEntity?>(entity)
            : ValueTask.FromResult<TEntity?>(null);
    }
}
