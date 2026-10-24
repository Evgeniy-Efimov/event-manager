using EventManager.Application.Services;
using EventManager.Application.Services.Interfaces;
using EventManager.Domain.Models;

namespace EventManager.UnitTests.Fixtures;

public class EventServiceFixture : BaseFixture
{
    public IEventService CreateEventService() => new EventService(
        CreateTestAppDbContext(),
        new SemaphoreLockProvider<Event>(),
        DateTimeProvider);
}