using EventManager.Domain.Enums;

namespace EventManager.Application.Models.DTO.Booking;

public record BookingDto(
    Guid Id,
    Guid EventId,
    BookingStatus Status,
    DateTime CreatedAt,
    DateTime? ProcessedAt
);
