namespace EventManager.Domain.Models;

public class Event : BaseEntity
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public int TotalSeats { get; set; }
    public int AvailableSeats { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public List<Booking> Bookings { get; set; } = [];

    private Event() { }

    public Event(
        string title,
        string? description,
        DateTime startAt,
        DateTime endAt,
        int totalSeats,
        int? availableSeats = null,
        DateTime? createdAt = null,
        DateTime? updatedAt = null,
        Guid? id = null) : base(id, createdAt, updatedAt)
    {
        Title = title;
        Description = description;
        TotalSeats = totalSeats;
        AvailableSeats = availableSeats ?? totalSeats;
        StartAt = startAt;
        EndAt = endAt;
    }

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
