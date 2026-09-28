using EventManager.Domain.Enums;

namespace EventManager.Domain.Models;

public class Booking(
    Guid eventId,
    BookingStatus status,
    DateTime? processedAt = null,
    DateTime? createdAt = null,
    DateTime? updatedAt = null,
    Guid? id = null) : BaseEntity(id, createdAt, updatedAt)
{
    public Guid EventId { get; set; } = eventId;
    public BookingStatus Status { get; private set; } = status;
    public DateTime? ProcessedAt { get; private set; } = processedAt;

    public void Confirm(DateTime processedAt) => ChangeStatus(processedAt, BookingStatus.Confirmed);

    public void Reject(DateTime processedAt) => ChangeStatus(processedAt, BookingStatus.Rejected);

    private void ChangeStatus(DateTime processedAt, BookingStatus status)
    {
        Status = status;
        ProcessedAt = processedAt;
        UpdatedAt = processedAt;
    }
}
