using EventManager.Domain.Enums;

namespace EventManager.Domain.Models;

public class Booking(Guid eventId, BookingStatus status, DateTime? processedAt = null, Guid? id = null) : BaseEntity(id)
{
    public Guid EventId { get; set; } = eventId;
    public BookingStatus Status { get; set; } = status;
    public DateTime? ProcessedAt { get; set; } = processedAt;
}
