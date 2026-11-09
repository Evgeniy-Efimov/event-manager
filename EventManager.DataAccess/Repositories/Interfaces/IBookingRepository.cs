using EventManager.Domain.Models;

namespace EventManager.DataAccess.Repositories.Interfaces;

public interface IBookingRepository
{
    Task<Booking> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Booking booking, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
