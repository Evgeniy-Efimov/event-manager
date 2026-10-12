using EventManager.Application.Constants;
using EventManager.Application.Extensions.Mapping.Booking;
using EventManager.Application.Extensions.Mapping.Events;
using EventManager.Application.Models.DTO.Booking;
using EventManager.Application.Models.Exceptions;
using EventManager.Application.Services.Interfaces;
using EventManager.Domain.Enums;
using EventManager.Domain.Models;
using Microsoft.Extensions.Logging;

namespace EventManager.Application.Services;

public class BookingService(
    IRepository<Booking> repository,
    IQueue<Booking> queue,
    ILockProvider<Event> eventLock,
    IEventService eventService,
    IDateTimeProvider dateTimeProvider,
    ILogger<BookingService> logger) : IBookingService
{
    public async Task<BookingDto> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return (await GetDomainAsync(id, cancellationToken)).ToDto();
    }

    private async Task<Booking> GetDomainAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return (await repository.GetAsync(id, cancellationToken))
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

            await eventService.UpdateWithoutLockAsync(@event, cancellationToken);

            try
            {
                await repository.CreateAsync(booking, cancellationToken);
            }
            catch
            {
                await TryRollbackBookingAsync(@event);
                throw;
            }

            try
            {
                await queue.EnqueueAsync(booking, cancellationToken);
            }
            catch
            {
                await TryRollbackBookingAsync(@event, booking);
                throw;
            }
        }

        return booking.ToDto();
    }

    private async Task TryRollbackBookingAsync(Event @event, Booking? booking = null)
    {
        try
        {
            if (booking != null)
            {
                await repository.DeleteAsync(booking.Id);
            }

            @event.ReleaseSeats();
            await eventService.UpdateWithoutLockAsync(@event);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to rollback seats for event {EventId}", @event.Id);
        }
    }

    public async Task<BookingDto> ConfirmAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var booking = await GetDomainAsync(id, cancellationToken);

        return await ProcessAsync(booking, (booking) => booking.Confirm(dateTimeProvider.Now), cancellationToken);
    }

    public async Task<BookingDto> RejectAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var booking = await GetDomainAsync(id, cancellationToken);
        var result = await ProcessAsync(booking, (booking) => booking.Reject(dateTimeProvider.Now), cancellationToken);

        await using (await eventLock.LockAsync(cancellationToken))
        {
            var @event = await eventService.GetDomainAsync(booking.EventId, cancellationToken);
            @event.ReleaseSeats();
            await eventService.UpdateWithoutLockAsync(@event, cancellationToken);
        }

        return result;
    }

    private async Task<BookingDto> ProcessAsync(Booking booking, Action<Booking> process, CancellationToken cancellationToken = default)
    {
        if (booking.Status != BookingStatus.Pending)
            throw new ValidationException($"Can't process booking in status '{booking.Status}'");

        process(booking);

        if (!await repository.UpdateAsync(booking, cancellationToken))
            throw new InvalidOperationException($"Failed to update booking '{booking.Id}'");

        return booking.ToDto();
    }
}
