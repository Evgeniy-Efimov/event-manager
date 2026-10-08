using EventManager.Application.Services;
using EventManager.Application.Services.Interfaces;
using EventManager.Domain.Enums;
using EventManager.Domain.Models;

namespace EventManager.UnitTests.Fixtures;

public class BookingServiceFixture : EventServiceFixture
{
    public IBookingService GetBookingService(IEventService? eventService = null) => new BookingService(
        new InMemoryRepository<Booking>(TestBookings.ToDictionary(b => b.Id)), eventService ?? EventService, DateTimeProvider);

    public static Booking[] TestBookings =>
    [
        new (
            eventId: Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
            BookingStatus.Pending,
            processedAt: null,
            createdAt: Today,
            updatedAt: Today,
            id: Guid.Parse("3f8a1c2e-9b4d-4e6a-8f1b-2c7d5e9a0b3f")
        ),
        new (
            eventId: Guid.Parse("9c858901-8a57-4791-81fe-4c455b099bc9"),
            BookingStatus.Pending,
            processedAt: null,
            createdAt: Today.AddDays(-1),
            updatedAt: Today.AddDays(-1),
            id: Guid.Parse("7b2e9d4a-1c5f-4a8b-9e3d-6f0a2b7c4d18")
        ),
        new (
            eventId: Guid.Parse("3d4e5f6a-7b8c-4d9e-a0b1-c2d3e4f5a6b7"),
            BookingStatus.Pending,
            processedAt: null,
            createdAt: Today.AddDays(-7),
            updatedAt: Today.AddDays(-6),
            id: Guid.Parse("c4d8f1a6-3e7b-4c2d-8a5f-1b9e6d3c7a20")
        ),
        new (
            eventId: Guid.Parse("550e8400-e29b-41d4-a716-446655440000"),
            BookingStatus.Confirmed,
            processedAt: Today.AddDays(-2),
            createdAt: Today.AddDays(-5),
            updatedAt: Today.AddDays(-2),
            id: Guid.Parse("9a6c3e8b-5d1f-4b7a-2c9e-4f8b1d6a3c57")
        ),
        new (
            eventId: Guid.Parse("6ba7b810-9dad-41d1-80b4-00c04fd430c8"),
            BookingStatus.Confirmed,
            processedAt: Today.AddHours(-3),
            createdAt: Today.AddDays(-1),
            updatedAt: Today.AddHours(-3),
            id: Guid.Parse("1e5b9d2c-7a4f-4e8b-3d6c-9f2a5b8d1e74")
        ),
        new (
            eventId: Guid.Parse("16fd2706-8baf-433b-82eb-8c7fada847da"),
            BookingStatus.Rejected,
            processedAt: Today.AddDays(-10),
            createdAt: Today.AddDays(-15),
            updatedAt: Today.AddDays(-10),
            id: Guid.Parse("6d3f8a1e-2c9b-4a5d-7e1f-3b8c6a9d2f45")
        ),
        new (
            eventId: Guid.Parse("886313e1-3b8a-437b-9b41-86a2e0e0e0e0"),
            BookingStatus.Rejected,
            processedAt: Today.AddHours(-6),
            createdAt: Today.AddDays(-2),
            updatedAt: Today.AddHours(-6),
            id: Guid.Parse("b7c2e5a9-4f1d-4b8e-6a3c-8d1f5b9e2a67")
        ),
        new (
            eventId: Guid.Parse("1b4e28ba-2fa1-41d3-8b4a-9c5d6e7f8a9b"),
            BookingStatus.Confirmed,
            processedAt: Today.AddMinutes(-45),
            createdAt: Today.AddHours(-2),
            updatedAt: Today.AddMinutes(-45),
            id: Guid.Parse("2f9a4c7e-8b3d-4e1a-5c6f-7d2b9a4e8c31")
        ),
        new (
            eventId: Guid.Parse("d3b07384-d9a0-4c9b-8e2f-1a7c6b5d4e3f"),
            BookingStatus.Rejected,
            processedAt: Today.AddDays(-59),
            createdAt: Today.AddDays(-60),
            updatedAt: Today.AddDays(-59),
            id: Guid.Parse("8e1d6b3f-5a9c-4d2e-7b4a-1c8f3e6d9b52")
        ),
        new (
            eventId: Guid.Parse("2c5e8a1f-6b9d-4e3a-7c2f-9b4d8a1e5c7f"),
            BookingStatus.Pending,
            processedAt: null,
            createdAt: Today.AddMinutes(-30),
            updatedAt: Today.AddMinutes(-30),
            id: Guid.Parse("4c7f2a9e-6d1b-4e8a-3f5c-9b2e7d1a4f83")
        )
    ];
}
