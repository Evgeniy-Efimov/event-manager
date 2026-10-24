using EventManager.Domain.Enums;

namespace EventManager.Domain.Models;

public class Booking : BaseEntity
{
    public Guid EventId { get; set; }
    public Event Event { get; set; } = null!;
    public BookingStatus Status { get; private set; }
    public DateTime? ProcessedAt { get; private set; }

    private Booking() { }

    public Booking(
        Guid eventId,
        BookingStatus status,
        DateTime? processedAt = null,
        DateTime? createdAt = null,
        DateTime? updatedAt = null,
        Guid? id = null) : base(id, createdAt, updatedAt)
    {
        EventId = eventId;
        Status = status;
        ProcessedAt = processedAt;
    }

    public void Confirm(DateTime processedAt) => ChangeStatus(processedAt, BookingStatus.Confirmed);

    public void Reject(DateTime processedAt) => ChangeStatus(processedAt, BookingStatus.Rejected);

    private void ChangeStatus(DateTime processedAt, BookingStatus status)
    {
        Status = status;
        ProcessedAt = processedAt;
        UpdatedAt = processedAt;
    }
}
