namespace EventManager.Domain.Models;

public class Event(
    string title,
    string? description,
    DateTime startAt,
    DateTime endAt,
    int totalSeats,
    int? availableSeats = null,
    DateTime? createdAt = null,
    DateTime? updatedAt = null,
    Guid? id = null) : BaseEntity(id, createdAt, updatedAt)
{
    public string Title { get; set; } = title;
    public string? Description { get; set; } = description;
    public int TotalSeats { get; set; } = totalSeats;
    public int AvailableSeats { get; set; } = availableSeats ?? totalSeats;
    public DateTime StartAt { get; set; } = startAt;
    public DateTime EndAt { get; set; } = endAt;

    public bool TryReserveSeats(int count = 1)
    {
        var updatedAvailableSeats = AvailableSeats - count;

        if (updatedAvailableSeats < 0)
            return false;

        AvailableSeats = updatedAvailableSeats;
        return true;
    }

    public void ReleaseSeats(int count = 1)
    {
        var updatedAvailableSeats = AvailableSeats + count;
        AvailableSeats = updatedAvailableSeats > TotalSeats
            ? TotalSeats
            : updatedAvailableSeats;
    }
}
