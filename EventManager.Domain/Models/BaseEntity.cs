namespace EventManager.Domain.Models;

public abstract class BaseEntity(Guid? id = null)
{
    public Guid Id { get; init; } = id ?? Guid.NewGuid();
}
