namespace EventManager.Domain.Models;

public class Event(
    string title,
    string? description,
    DateTime startAt,
    DateTime endAt,
    DateTime? createdAt = null,
    DateTime? updatedAt = null,
    Guid? id = null) : BaseEntity(id, createdAt, updatedAt)
{
    public string Title { get; set; } = title;
    public string? Description { get; set; } = description;
    public DateTime StartAt { get; set; } = startAt;
    public DateTime EndAt { get; set; } = endAt;
}
