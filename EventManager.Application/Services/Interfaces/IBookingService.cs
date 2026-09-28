using EventManager.Application.Models.DTO.Booking;

namespace EventManager.Application.Services.Interfaces;

public interface IBookingService
{
    Task<BookingDto> Get(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BookingDto>> GetPendingBatch(int? batchSize = null, CancellationToken cancellationToken = default);
    Task<BookingDto> Create(Guid eventId, CancellationToken cancellationToken = default);
    Task<BookingDto> Confirm(Guid id, CancellationToken cancellationToken = default);
    Task<BookingDto> Reject(Guid id, CancellationToken cancellationToken = default);
}
