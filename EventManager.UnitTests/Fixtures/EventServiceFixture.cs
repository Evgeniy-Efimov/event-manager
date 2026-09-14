using EventManager.Application.Services;
using EventManager.Application.Services.Interfaces;
using EventManager.Domain.Models;

namespace EventManager.UnitTests.Fixtures;

public class EventServiceFixture
{
    public IEventService EventService => new EventService(new InMemoryRepository<Event>(TestEvents.ToDictionary(e => e.Id)));

    public readonly List<Event> TestEvents =
    [
        new ("Concert", "Music festival", DateTime.Today.AddDays(1), DateTime.Today.AddDays(1).AddHours(3), Guid.Parse("a1b2c3d4-e5f6-4a7b-8c9d-0e1f2a3b4c5d")),
        new ("IT Seminar", null, DateTime.Today.AddDays(2), DateTime.Today.AddDays(2).AddHours(4), Guid.Parse("b2c3d4e5-f6a7-4b8c-9d0e-1f2a3b4c5d6e")),
        new ("Yoga Evening", "Meditation and exercises", DateTime.Today.AddDays(7), DateTime.Today.AddDays(7).AddHours(1), Guid.Parse("c3d4e5f6-a7b8-4c9d-0e1f-2a3b4c5d6e7f")),
        new ("Theater Evening", null, DateTime.Today.AddDays(3), DateTime.Today.AddDays(3).AddHours(2), Guid.Parse("d4e5f6a7-b8c9-4d0e-1f2a-3b4c5d6e7f8a")),
        new ("Art Exhibition", "Photo exposition", DateTime.Today.AddDays(5), DateTime.Today.AddDays(5).AddDays(3), Guid.Parse("e5f6a7b8-c9d0-4e1f-2a3b-4c5d6e7f8a9b")),
        new ("Morning Run", "", DateTime.Today.AddHours(7), DateTime.Today.AddHours(8.5), Guid.Parse("f6a7b8c9-d0e1-4f2a-3b4c-5d6e7f8a9b0c")),
        new ("Movie Screening", null, DateTime.Today.AddDays(4), DateTime.Today.AddDays(4).AddHours(2.5), Guid.Parse("a7b8c9d0-e1f2-4a3b-4c5d-6e7f8a9b0c1d")),
        new ("Drawing Masterclass", "Watercolor for beginners", DateTime.Today.AddDays(6), DateTime.Today.AddDays(6).AddHours(3), Guid.Parse("b8c9d0e1-f2a3-4b4c-5d6e-7f8a9b0c1d2e")),
        new ("Quiz Day", null, DateTime.Today.AddHours(19), DateTime.Today.AddHours(21), Guid.Parse("c9d0e1f2-a3b4-4c5d-6e7f-8a9b0c1d2e3f")),
        new ("Football Match", "Champions League", DateTime.Today.AddDays(8), DateTime.Today.AddDays(8).AddHours(2), Guid.Parse("d0e1f2a3-b4c5-4d6e-7f8a-9b0c1d2e3f4a")),
        new ("Birthday Party", "", DateTime.Today.AddDays(10), DateTime.Today.AddDays(10).AddHours(3), Guid.Parse("e1f2a3b4-c5d6-4e7f-8a9b-0c1d2e3f4a5b")),
        new ("Demo Showcase", null, DateTime.Today.AddDays(5), DateTime.Today.AddDays(5).AddHours(4), Guid.Parse("f2a3b4c5-d6e7-4f8a-9b0c-1d2e3f4a5b6c")),
        new ("Wine Tasting", "Italian wines", DateTime.Today.AddDays(9), DateTime.Today.AddDays(9).AddHours(2.5), Guid.Parse("a3b4c5d6-e7f8-4a9b-0c1d-2e3f4a5b6c7d")),
        new ("Fitness Workout", null, DateTime.Today.AddHours(18), DateTime.Today.AddHours(19.5), Guid.Parse("b4c5d6e7-f8a9-4b0c-1d2e-3f4a5b6c7d8e")),
        new ("Marketing Lecture", "Digital Marketing 2026", DateTime.Today.AddDays(4), DateTime.Today.AddDays(4).AddHours(2), Guid.Parse("c5d6e7f8-a9b0-4c1d-2e3f-4a5b6c7d8e9f"))
    ];
}