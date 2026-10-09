using EventManager.Application.Models.DTO.Booking;
using EventManager.Domain.Models;

namespace EventManager.Application.Services.Interfaces;

public interface IBookingService
{
    Task<BookingDto> GetAsync(Guid id, CancellationToken cancellationToken = default);
    ValueTask<IReadOnlyList<Booking>> GetPendingBookingsAsync(int? batchSize = null, CancellationToken cancellationToken = default);
    Task<BookingDto> CreateAsync(Guid eventId, CancellationToken cancellationToken = default);
    Task<BookingDto> ConfirmAsync(Guid id, CancellationToken cancellationToken = default);
    Task<BookingDto> RejectAsync(Guid id, CancellationToken cancellationToken = default);
}
