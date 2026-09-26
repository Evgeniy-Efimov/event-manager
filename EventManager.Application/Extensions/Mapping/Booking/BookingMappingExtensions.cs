using EventManager.Application.Models.DTO.Booking;
using BookingModel = EventManager.Domain.Models.Booking;

namespace EventManager.Application.Extensions.Mapping.Booking;

public static class BookingMappingExtensions
{
    public static BookingDto ToDto(this BookingModel booking)
    {
        return new BookingDto(
            booking.Id,
            booking.EventId,
            booking.Status.ToString(),
            booking.ProcessedAt,
            booking.CreatedAt,
            booking.UpdatedAt);
    }
}
