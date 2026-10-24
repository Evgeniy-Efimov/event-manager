using EventManager.Application.Constants;
using EventManager.Application.Extensions.Mapping.Booking;
using EventManager.Application.Extensions.Mapping.Events;
using EventManager.Application.Models.DTO.Booking;
using EventManager.Application.Models.Exceptions;
using EventManager.Application.Services.Interfaces;
using EventManager.Domain.DbContexts;
using EventManager.Domain.Enums;
using EventManager.Domain.Models;

namespace EventManager.Application.Services;

public class BookingService(
    AppDbContext context,
    IQueue<Booking> queue,
    ILockProvider<Event> eventLock,
    IEventService eventService,
    IDateTimeProvider dateTimeProvider) : IBookingService
{
    public async Task<BookingDto> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return (await GetDomainAsync(id, cancellationToken)).ToDto();
    }

    private async Task<Booking> GetDomainAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return (await context.Bookings.FindAsync([id], cancellationToken))
            ?? throw new NotFoundException($"Booking '{id}' not found");
    }

    public async ValueTask<IReadOnlyList<Booking>> GetPendingBookingsAsync(
        int? batchSize = null, CancellationToken cancellationToken = default)
    {
        batchSize ??= BookingConstants.DefaultPendingBatchSize;
        var batch = new List<Booking>();

        while (batch.Count < batchSize)
        {
            var booking = await queue.ReadAsync(cancellationToken);

            if (booking is null)
                break;

            batch.Add(booking);
        }

        return batch;
    }

    public async Task<BookingDto> CreateAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        var booking = new Booking(eventId, BookingStatus.Pending, createdAt: dateTimeProvider.Now);

        await using (await eventLock.LockAsync(cancellationToken))
        {
            var @event = await eventService.GetDomainAsync(eventId, cancellationToken);

            if (!@event.TryReserveSeats())
                throw new NoAvailableSeatsException();

            await context.Bookings.AddAsync(booking, cancellationToken);
            await queue.EnqueueAsync(booking, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        return booking.ToDto();
    }

    public async Task<BookingDto> ConfirmAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var booking = await GetDomainAsync(id, cancellationToken);
        booking = Process(booking, (booking) => booking.Confirm(dateTimeProvider.Now));
        await context.SaveChangesAsync(cancellationToken);

        return booking.ToDto();
    }

    public async Task<BookingDto> RejectAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var booking = await GetDomainAsync(id, cancellationToken);
        booking = Process(booking, (booking) => booking.Reject(dateTimeProvider.Now));

        await using (await eventLock.LockAsync(cancellationToken))
        {
            var @event = await eventService.GetDomainAsync(booking.EventId, cancellationToken);
            @event.ReleaseSeats();
            await context.SaveChangesAsync(cancellationToken);
        }

        return booking.ToDto();
    }

    private static Booking Process(Booking booking, Action<Booking> process)
    {
        if (booking.Status != BookingStatus.Pending)
            throw new ValidationException($"Can't process booking in status '{booking.Status}'");

        process(booking);
        
        return booking;
    }
}
