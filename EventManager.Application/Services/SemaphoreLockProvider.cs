using EventManager.Application.Services.Interfaces;

namespace EventManager.Application.Services;

public class SemaphoreLockProvider<TEntity> : ILockProvider<TEntity>
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public async Task<IAsyncDisposable> LockAsync(CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);
        return new SemaphoreEntityLockProviderReleaser(_semaphore);
    }

    private class SemaphoreEntityLockProviderReleaser(SemaphoreSlim semaphore) : IAsyncDisposable, IDisposable
    {
        private SemaphoreSlim? _semaphore = semaphore;

        public void Dispose()
        {
            Interlocked.Exchange(ref _semaphore, null)?.Release();
        }

        public ValueTask DisposeAsync()
        {
            Dispose();
            return ValueTask.CompletedTask;
        }
    }
}
