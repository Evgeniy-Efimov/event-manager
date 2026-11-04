using EventManager.Application.Models.Exceptions;
using EventManager.Application.Services.Interfaces;
using EventManager.Domain.DbContexts;
using EventManager.Domain.Models;

namespace EventManager.Application.Services;

public class BookingRepository(AppDbContext context) : IBookingRepository
{
    public async Task<Booking> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return (await context.Bookings.FindAsync([id], cancellationToken))
            ?? throw new NotFoundException($"Booking '{id}' not found");
    }

    public Task AddAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        return context.Bookings.AddAsync(booking, cancellationToken).AsTask();
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return context.SaveChangesAsync(cancellationToken);
    }
}
