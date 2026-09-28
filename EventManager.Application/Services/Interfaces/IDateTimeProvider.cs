namespace EventManager.Application.Services.Interfaces;

public interface IDateTimeProvider
{
    public DateTime Now { get; }
    public DateTime Today { get; }
}
