using EventManager.Domain.Models;

namespace EventManager.Domain.Repositories;

public interface IBookingRepository
{
    Task<Booking> GetAsync(Guid id, bool withEvent = false, CancellationToken cancellationToken = default);
    Task AddAsync(Booking booking, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
