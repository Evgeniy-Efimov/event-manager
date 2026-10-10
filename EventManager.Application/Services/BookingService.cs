using EventManager.Application.Constants;
using EventManager.Application.Extensions.Mapping.Booking;
using EventManager.Application.Extensions.Mapping.Events;
using EventManager.Application.Models.DTO.Booking;
using EventManager.Application.Models.Exceptions;
using EventManager.Application.Services.Interfaces;
using EventManager.Domain.Enums;
using EventManager.Domain.Models;

namespace EventManager.Application.Services;

public class BookingService(
    IRepository<Booking> repository,
    IQueue<Booking> queue,
    IEventService eventService,
    IDateTimeProvider dateTimeProvider) : IBookingService
{
    private readonly SemaphoreSlim _eventSemaphore = new(1, 1);

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

        await _eventSemaphore.WaitAsync(cancellationToken);
        try
        {
            var @event = await eventService.GetDomainAsync(eventId, cancellationToken);

            if (!@event.TryReserveSeats())
                throw new NoAvailableSeatsException(eventId);

            await eventService.UpdateAsync(@event, cancellationToken);
        }
        finally
        {
            _eventSemaphore.Release();
        }

        await repository.CreateAsync(booking, cancellationToken);
        await queue.EnqueueAsync(booking, cancellationToken);

        return booking.ToDto();
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

        await _eventSemaphore.WaitAsync(cancellationToken);
        try
        {
            var @event = await eventService.GetDomainAsync(booking.EventId, cancellationToken);
            @event.ReleaseSeats();

            await eventService.UpdateAsync(@event, cancellationToken);
        }
        finally
        {
            _eventSemaphore.Release();
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
