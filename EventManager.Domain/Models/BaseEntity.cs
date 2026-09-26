namespace EventManager.Domain.Models;

public abstract class BaseEntity
{
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; set; }

    protected BaseEntity(Guid? id = null, DateTime? createdAt = null, DateTime? updatedAt = null)
    {
        Id = id ?? Guid.NewGuid();
        CreatedAt = createdAt ?? DateTime.UtcNow;
        UpdatedAt = updatedAt ?? CreatedAt;
    }
}
