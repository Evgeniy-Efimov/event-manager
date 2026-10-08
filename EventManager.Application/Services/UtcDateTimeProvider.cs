using EventManager.Application.Services.Interfaces;

namespace EventManager.Application.Services;

public class UtcDateTimeProvider : IDateTimeProvider
{
    public DateTime Now => DateTime.UtcNow;

    public DateTime Today => DateTime.UtcNow.Date;
}
