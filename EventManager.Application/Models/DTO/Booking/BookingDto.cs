using EventManager.Domain.Enums;

namespace EventManager.Application.Models.DTO.Booking;

public record BookingDto(
    Guid Id,
    Guid EventId,
    string Status,
    DateTime? ProcessedAt,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
