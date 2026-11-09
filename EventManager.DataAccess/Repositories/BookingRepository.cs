using EventManager.DataAccess.DbContexts;
using EventManager.DataAccess.Models.Exceptions;
using EventManager.DataAccess.Repositories.Interfaces;
using EventManager.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EventManager.DataAccess.Repositories;

public class BookingRepository(AppDbContext context) : IBookingRepository
{
    public async Task<Booking> GetAsync(Guid id, bool withEvent = false, CancellationToken cancellationToken = default)
    {
        var query = context.Bookings.AsQueryable();

        if (withEvent)
            query = query.Include(b => b.Event);

        return (await query.FirstOrDefaultAsync(b => b.Id == id, cancellationToken))
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
