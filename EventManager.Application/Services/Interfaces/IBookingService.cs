using EventManager.Application.Models.DTO.Booking;

namespace EventManager.Application.Services.Interfaces;

public interface IBookingService
{
    Task<BookingDto> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<BookingDto> CreateAsync(Guid eventId, CancellationToken cancellationToken = default);
    Task<BookingDto> ConfirmAsync(Guid id, CancellationToken cancellationToken = default);
    Task<BookingDto> RejectAsync(Guid id, CancellationToken cancellationToken = default);
}
