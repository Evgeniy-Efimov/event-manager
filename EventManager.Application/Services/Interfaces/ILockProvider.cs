namespace EventManager.Application.Services.Interfaces;

public interface ILockProvider<TEntity>
{
    Task<IAsyncDisposable> LockAsync(CancellationToken cancellationToken = default);
}
