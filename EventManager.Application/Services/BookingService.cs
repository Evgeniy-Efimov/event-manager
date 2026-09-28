using EventManager.Application.Constants;
using EventManager.Application.Extensions.Mapping.Booking;
using EventManager.Application.Models.DTO.Booking;
using EventManager.Application.Models.Exceptions;
using EventManager.Application.Services.Interfaces;
using EventManager.Domain.Enums;
using EventManager.Domain.Models;

namespace EventManager.Application.Services;

public class BookingService(
    IRepository<Booking> repository,
    IEventService eventService,
    IDateTimeProvider dateTimeProvider) : IBookingService
{
    public async Task<BookingDto> Get(Guid id, CancellationToken cancellationToken = default)
    {
        return (await GetDomain(id, cancellationToken)).ToDto();
    }

    public async Task<Booking> GetDomain(Guid id, CancellationToken cancellationToken = default)
    {
        return (await repository.Get(id, cancellationToken))
            ?? throw new NotFoundException($"Booking '{id}' not found");
    }

    public async Task<IReadOnlyList<BookingDto>> GetPendingBatch(int? batchSize = null, CancellationToken cancellationToken = default)
    {
        batchSize ??= BookingConstants.DefaultPendingBatchSize;

        return (await repository.GetList(cancellationToken))
            .Where(b => b.Status == BookingStatus.Pending)
            .OrderBy(b => b.CreatedAt)
            .Take(batchSize.Value).ToDtoList();
    }

    public async Task<BookingDto> Create(Guid eventId, CancellationToken cancellationToken = default)
    {
        var @event = await eventService.Get(eventId, cancellationToken);
        var booking = new Booking(@event.Id, BookingStatus.Pending, createdAt: dateTimeProvider.Now);
        await repository.Create(booking, cancellationToken);

        return booking.ToDto();
    }

    public Task<BookingDto> Confirm(Guid id, CancellationToken cancellationToken = default) =>
        Process(id, (booking) => booking.Confirm(dateTimeProvider.Now), cancellationToken);

    public Task<BookingDto> Reject(Guid id, CancellationToken cancellationToken = default) =>
        Process(id, (booking) => booking.Reject(dateTimeProvider.Now), cancellationToken);

    private async Task<BookingDto> Process(Guid id, Action<Booking> process, CancellationToken cancellationToken = default)
    {
        var booking = await GetDomain(id, cancellationToken);

        if (booking.Status != BookingStatus.Pending)
            throw new ValidationException($"Can't process booking in status '{booking.Status}'");

        process(booking);

        if (!await repository.Update(booking, cancellationToken))
            throw new InvalidOperationException($"Failed to update booking '{booking.Id}'");

        return booking.ToDto();
    }
}
