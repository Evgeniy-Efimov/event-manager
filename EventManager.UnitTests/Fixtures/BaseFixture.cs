using EventManager.Application.Services.Interfaces;
using EventManager.Domain.DbContexts;
using EventManager.Domain.Enums;
using EventManager.Domain.Models;
using EventManager.UnitTests.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace EventManager.UnitTests.Fixtures;

public abstract class BaseFixture
{
    public Mock<IDateTimeProvider> DateTimeProviderMock { get; } = new();
    protected IDateTimeProvider DateTimeProvider => DateTimeProviderMock.Object;

    public BaseFixture()
    {
        Assert.Equal(TestEventsCount, TestEvents.Length);
        DateTimeProviderMock.Setup(p => p.Now).Returns(DateTime.UtcNow);
        DateTimeProviderMock.Setup(p => p.Today).Returns(DateTime.UtcNow.Date);
    }

    protected static TestAppDbContext CreateTestAppDbContext(string? dbName = null)
    {
        dbName ??= Guid.NewGuid().ToString();
        var context = new TestAppDbContext(new DbContextOptionsBuilder<AppDbContext>().Options, dbName, TestEvents, TestBookings);
        context.Database.EnsureCreated();

        return context;
    }

    public virtual IServiceProvider CreateServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddScoped<AppDbContext>(_ => CreateTestAppDbContext());
        return services.BuildServiceProvider();
    }

    public static readonly DateTime Today = DateTime.Today;

    public const int TestEventsCount = 15;
    public static Event[] TestEvents =>
    [
        new (
            "Concert",
            "Music festival",
            startAt: Today.AddDays(1),
            endAt: Today.AddDays(1).AddHours(3),
            totalSeats: 1000,
            createdAt: Today.AddDays(1).AddDays(-5),
            updatedAt: Today.AddDays(1).AddDays(-1),
            id: Guid.Parse("a1b2c3d4-e5f6-4a7b-8c9d-0e1f2a3b4c5d")
        ),
        new (
            "IT Seminar",
            null,
            startAt: Today.AddDays(2),
            endAt: Today.AddDays(2).AddHours(4),
            totalSeats: 200,
            createdAt: Today.AddDays(2).AddDays(-7),
            updatedAt: Today.AddDays(2).AddDays(-2),
            id: Guid.Parse("b2c3d4e5-f6a7-4b8c-9d0e-1f2a3b4c5d6e")
        ),
        new (
            "Yoga Evening",
            "Meditation and exercises",
            startAt: Today.AddDays(7),
            endAt: Today.AddDays(7).AddHours(1),
            totalSeats: 30,
            createdAt: Today.AddDays(7).AddDays(-10),
            updatedAt: Today.AddDays(7).AddDays(-3),
            id: Guid.Parse("c3d4e5f6-a7b8-4c9d-0e1f-2a3b4c5d6e7f")
        ),
        new (
            "Theater Evening",
            null,
            startAt: Today.AddDays(3),
            endAt: Today.AddDays(3).AddHours(2),
            totalSeats: 100,
            createdAt: Today.AddDays(3).AddDays(-4),
            updatedAt: Today.AddDays(3).AddDays(-1),
            id: Guid.Parse("d4e5f6a7-b8c9-4d0e-1f2a-3b4c5d6e7f8a")
        ),
        new (
            "Art Exhibition",
            "Photo exposition",
            startAt: Today.AddDays(5),
            endAt: Today.AddDays(5).AddDays(3),
            totalSeats: 300,
            createdAt: Today.AddDays(5).AddDays(-14),
            updatedAt: Today.AddDays(5).AddDays(-6),
            id: Guid.Parse("e5f6a7b8-c9d0-4e1f-2a3b-4c5d6e7f8a9b")
        ),
        new (
            "Morning Run",
            "",
            startAt: Today.AddHours(7),
            endAt: Today.AddHours(8.5),
            totalSeats: 50,
            createdAt: Today.AddHours(7).AddDays(-2),
            updatedAt: Today.AddHours(7).AddDays(-1),
            id: Guid.Parse("f6a7b8c9-d0e1-4f2a-3b4c-5d6e7f8a9b0c")
        ),
        new (
            "Movie Screening",
            null,
            startAt: Today.AddDays(4),
            endAt: Today.AddDays(4).AddHours(2.5),
            totalSeats: 100,
            createdAt: Today.AddDays(4).AddDays(-5),
            updatedAt: Today.AddDays(4).AddDays(-2),
            id: Guid.Parse("a7b8c9d0-e1f2-4a3b-4c5d-6e7f8a9b0c1d")
        ),
        new (
            "Drawing Masterclass",
            "Watercolor for beginners",
            startAt: Today.AddDays(6),
            endAt: Today.AddDays(6).AddHours(3),
            totalSeats: 50,
            createdAt: Today.AddDays(6).AddDays(-8),
            updatedAt: Today.AddDays(6).AddDays(-4),
            id: Guid.Parse("b8c9d0e1-f2a3-4b4c-5d6e-7f8a9b0c1d2e")
        ),
        new (
            "Quiz Day",
            null,
            startAt: Today.AddHours(19),
            endAt: Today.AddHours(21),
            totalSeats: 20,
            createdAt: Today.AddHours(19).AddDays(-3),
            updatedAt: Today.AddHours(19).AddDays(-1),
            id: Guid.Parse("c9d0e1f2-a3b4-4c5d-6e7f-8a9b0c1d2e3f")
        ),
        new (
            "Football Match",
            "Champions League",
            startAt: Today.AddDays(8),
            endAt: Today.AddDays(8).AddHours(2),
            totalSeats: 8000,
            availableSeats: 7000,
            createdAt: Today.AddDays(8).AddDays(-12),
            updatedAt: Today.AddDays(8).AddDays(-5),
            id: Guid.Parse("d0e1f2a3-b4c5-4d6e-7f8a-9b0c1d2e3f4a")
        ),
        new (
            "Birthday Party",
            "",
            startAt: Today.AddDays(10),
            endAt: Today.AddDays(10).AddHours(3),
            totalSeats: 15,
            createdAt: Today.AddDays(10).AddDays(-15),
            updatedAt: Today.AddDays(10).AddDays(-7),
            id: Guid.Parse("e1f2a3b4-c5d6-4e7f-8a9b-0c1d2e3f4a5b")
        ),
        new (
            "Demo Showcase",
            null,
            startAt: Today.AddDays(5),
            endAt: Today.AddDays(5).AddHours(4),
            totalSeats: 10,
            createdAt: Today.AddDays(5).AddDays(-9),
            updatedAt: Today.AddDays(5).AddDays(-3),
            id: Guid.Parse("f2a3b4c5-d6e7-4f8a-9b0c-1d2e3f4a5b6c")
        ),
        new (
            "Wine Tasting",
            "Italian wines",
            startAt: Today.AddDays(9),
            endAt: Today.AddDays(9).AddHours(2.5),
            totalSeats: 50,
            createdAt: Today.AddDays(9).AddDays(-11),
            updatedAt: Today.AddDays(9).AddDays(-4),
            id: Guid.Parse("a3b4c5d6-e7f8-4a9b-0c1d-2e3f4a5b6c7d")
        ),
        new (
            "Fitness Workout",
            null,
            startAt: Today.AddHours(18),
            endAt: Today.AddHours(19.5),
            totalSeats: 40,
            availableSeats: 0,
            createdAt: Today.AddHours(18).AddDays(-6),
            updatedAt: Today.AddHours(18).AddDays(-2),
            id: Guid.Parse("b4c5d6e7-f8a9-4b0c-1d2e-3f4a5b6c7d8e")
        ),
        new (
            "Marketing Lecture",
            "Digital Marketing 2026",
            startAt: Today.AddDays(4),
            endAt: Today.AddDays(4).AddHours(2),
            totalSeats: 100,
            createdAt: Today.AddDays(4).AddDays(-10),
            updatedAt: Today.AddDays(4).AddDays(-3),
            id: Guid.Parse("c5d6e7f8-a9b0-4c1d-2e3f-4a5b6c7d8e9f")
        )
    ];

    public static Booking[] TestBookings =>
    [
        new (
            eventId: Guid.Parse("a1b2c3d4-e5f6-4a7b-8c9d-0e1f2a3b4c5d"),
            BookingStatus.Pending,
            processedAt: null,
            createdAt: Today,
            updatedAt: Today,
            id: Guid.Parse("3f8a1c2e-9b4d-4e6a-8f1b-2c7d5e9a0b3f")
        ),
        new (
            eventId: Guid.Parse("b2c3d4e5-f6a7-4b8c-9d0e-1f2a3b4c5d6e"),
            BookingStatus.Pending,
            processedAt: null,
            createdAt: Today.AddDays(-1),
            updatedAt: Today.AddDays(-1),
            id: Guid.Parse("7b2e9d4a-1c5f-4a8b-9e3d-6f0a2b7c4d18")
        ),
        new (
            eventId: Guid.Parse("b4c5d6e7-f8a9-4b0c-1d2e-3f4a5b6c7d8e"),
            BookingStatus.Pending,
            processedAt: null,
            createdAt: Today.AddDays(-7),
            updatedAt: Today.AddDays(-6),
            id: Guid.Parse("c4d8f1a6-3e7b-4c2d-8a5f-1b9e6d3c7a20")
        ),
        new (
            eventId: Guid.Parse("d4e5f6a7-b8c9-4d0e-1f2a-3b4c5d6e7f8a"),
            BookingStatus.Confirmed,
            processedAt: Today.AddDays(-2),
            createdAt: Today.AddDays(-5),
            updatedAt: Today.AddDays(-2),
            id: Guid.Parse("9a6c3e8b-5d1f-4b7a-2c9e-4f8b1d6a3c57")
        ),
        new (
            eventId: Guid.Parse("e5f6a7b8-c9d0-4e1f-2a3b-4c5d6e7f8a9b"),
            BookingStatus.Confirmed,
            processedAt: Today.AddHours(-3),
            createdAt: Today.AddDays(-1),
            updatedAt: Today.AddHours(-3),
            id: Guid.Parse("1e5b9d2c-7a4f-4e8b-3d6c-9f2a5b8d1e74")
        ),
        new (
            eventId: Guid.Parse("f6a7b8c9-d0e1-4f2a-3b4c-5d6e7f8a9b0c"),
            BookingStatus.Rejected,
            processedAt: Today.AddDays(-10),
            createdAt: Today.AddDays(-15),
            updatedAt: Today.AddDays(-10),
            id: Guid.Parse("6d3f8a1e-2c9b-4a5d-7e1f-3b8c6a9d2f45")
        ),
        new (
            eventId: Guid.Parse("a7b8c9d0-e1f2-4a3b-4c5d-6e7f8a9b0c1d"),
            BookingStatus.Rejected,
            processedAt: Today.AddHours(-6),
            createdAt: Today.AddDays(-2),
            updatedAt: Today.AddHours(-6),
            id: Guid.Parse("b7c2e5a9-4f1d-4b8e-6a3c-8d1f5b9e2a67")
        ),
        new (
            eventId: Guid.Parse("b8c9d0e1-f2a3-4b4c-5d6e-7f8a9b0c1d2e"),
            BookingStatus.Confirmed,
            processedAt: Today.AddMinutes(-45),
            createdAt: Today.AddHours(-2),
            updatedAt: Today.AddMinutes(-45),
            id: Guid.Parse("2f9a4c7e-8b3d-4e1a-5c6f-7d2b9a4e8c31")
        ),
        new (
            eventId: Guid.Parse("c9d0e1f2-a3b4-4c5d-6e7f-8a9b0c1d2e3f"),
            BookingStatus.Rejected,
            processedAt: Today.AddDays(-59),
            createdAt: Today.AddDays(-60),
            updatedAt: Today.AddDays(-59),
            id: Guid.Parse("8e1d6b3f-5a9c-4d2e-7b4a-1c8f3e6d9b52")
        ),
        new (
            eventId: Guid.Parse("d0e1f2a3-b4c5-4d6e-7f8a-9b0c1d2e3f4a"),
            BookingStatus.Pending,
            processedAt: null,
            createdAt: Today.AddMinutes(-30),
            updatedAt: Today.AddMinutes(-30),
            id: Guid.Parse("4c7f2a9e-6d1b-4e8a-3f5c-9b2e7d1a4f83")
        )
    ];
}
