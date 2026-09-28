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
    public async Task<BookingDto> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return (await GetDomainAsync(id, cancellationToken)).ToDto();
    }

    public async Task<Booking> GetDomainAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return (await repository.GetAsync(id, cancellationToken))
            ?? throw new NotFoundException($"Booking '{id}' not found");
    }

    public async Task<IReadOnlyList<BookingDto>> GetPendingBatchAsync(int? batchSize = null, CancellationToken cancellationToken = default)
    {
        batchSize ??= BookingConstants.DefaultPendingBatchSize;

        return (await repository.GetListAsync(cancellationToken))
            .Where(b => b.Status == BookingStatus.Pending)
            .OrderBy(b => b.CreatedAt)
            .Take(batchSize.Value).ToDtoList();
    }

    public async Task<BookingDto> CreateAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        var @event = await eventService.GetAsync(eventId, cancellationToken);
        var booking = new Booking(@event.Id, BookingStatus.Pending, createdAt: dateTimeProvider.Now);
        await repository.CreateAsync(booking, cancellationToken);

        return booking.ToDto();
    }

    public Task<BookingDto> ConfirmAsync(Guid id, CancellationToken cancellationToken = default) =>
        ProcessAsync(id, (booking) => booking.Confirm(dateTimeProvider.Now), cancellationToken);

    public Task<BookingDto> RejectAsync(Guid id, CancellationToken cancellationToken = default) =>
        ProcessAsync(id, (booking) => booking.Reject(dateTimeProvider.Now), cancellationToken);

    private async Task<BookingDto> ProcessAsync(Guid id, Action<Booking> process, CancellationToken cancellationToken = default)
    {
        var booking = await GetDomainAsync(id, cancellationToken);

        if (booking.Status != BookingStatus.Pending)
            throw new ValidationException($"Can't process booking in status '{booking.Status}'");

        process(booking);

        if (!await repository.UpdateAsync(booking, cancellationToken))
            throw new InvalidOperationException($"Failed to update booking '{booking.Id}'");

        return booking.ToDto();
    }
}
