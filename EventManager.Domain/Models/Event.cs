namespace EventManager.Domain.Models;

public class Event(string title, string? description, DateTime startAt, DateTime endAt, Guid? id = null) : BaseEntity(id)
{
    public string Title { get; set; } = title;
    public string? Description { get; set; } = description;
    public DateTime StartAt { get; set; } = startAt;
    public DateTime EndAt { get; set; } = endAt;
}
