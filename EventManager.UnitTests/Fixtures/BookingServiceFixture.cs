using EventManager.Application.Services;
using EventManager.Application.Services.Interfaces;
using EventManager.Domain.Enums;
using EventManager.Domain.Models;
using System.Threading.Channels;

namespace EventManager.UnitTests.Fixtures;

public class BookingServiceFixture : EventServiceFixture
{
    public IBookingService GetBookingService(IEventService? eventService = null, IQueue<Booking>? bookingQueue = null) => new BookingService(
        new InMemoryRepository<Booking>(TestBookings.ToDictionary(b => b.Id)),
        bookingQueue ?? new InMemoryQueue<Booking>(Channel.CreateUnbounded<Booking>()),
        eventService ?? EventService,
        DateTimeProvider);

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
