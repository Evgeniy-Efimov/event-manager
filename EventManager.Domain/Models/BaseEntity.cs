namespace EventManager.Domain.Models;

public abstract class BaseEntity(Guid? id = null, DateTime? createdAt = null)
{
    public Guid Id { get; init; } = id ?? Guid.NewGuid();
    public DateTime CreatedAt { get; init; } = createdAt ?? DateTime.UtcNow;
}
