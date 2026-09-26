using EventManager.Application.Services;
using EventManager.Application.Services.Interfaces;
using EventManager.Domain.Enums;
using EventManager.Domain.Models;

namespace EventManager.UnitTests.Fixtures;

public class BookingServiceFixture : EventServiceFixture
{
    public IBookingService BookingService => new BookingService(new InMemoryRepository<Booking>(
        TestBookings.ToDictionary(b => b.Id, b => new Booking(b.Id, b.Status, b.ProcessedAt, b.CreatedAt, b.UpdatedAt, b.Id))),
        EventService, DateTimeProvider);

    public readonly List<Booking> TestBookings =
    [
        new (Guid.NewGuid(), BookingStatus.Pending, null, DateTime.UtcNow, id: Guid.Parse("3f8a1c2e-9b4d-4e6a-8f1b-2c7d5e9a0b3f")),
        new (Guid.NewGuid(), BookingStatus.Pending, null, DateTime.UtcNow.AddDays(-1), id: Guid.Parse("7b2e9d4a-1c5f-4a8b-9e3d-6f0a2b7c4d18")),
        new (Guid.NewGuid(), BookingStatus.Pending, null, DateTime.UtcNow.AddDays(-7), id: Guid.Parse("c4d8f1a6-3e7b-4c2d-8a5f-1b9e6d3c7a20")),
        new (Guid.NewGuid(), BookingStatus.Confirmed, DateTime.UtcNow.AddDays(-2), DateTime.UtcNow.AddDays(-5), id: Guid.Parse("9a6c3e8b-5d1f-4b7a-2c9e-4f8b1d6a3c57")),
        new (Guid.NewGuid(), BookingStatus.Confirmed, DateTime.UtcNow.AddHours(-3), DateTime.UtcNow.AddDays(-1), id: Guid.Parse("1e5b9d2c-7a4f-4e8b-3d6c-9f2a5b8d1e74")),
        new (Guid.NewGuid(), BookingStatus.Rejected, DateTime.UtcNow.AddDays(-10), DateTime.UtcNow.AddDays(-15), id: Guid.Parse("6d3f8a1e-2c9b-4a5d-7e1f-3b8c6a9d2f45")),
        new (Guid.NewGuid(), BookingStatus.Rejected, DateTime.UtcNow.AddHours(-6), DateTime.UtcNow.AddDays(-2), id: Guid.Parse("b7c2e5a9-4f1d-4b8e-6a3c-8d1f5b9e2a67")),
        new (Guid.NewGuid(), BookingStatus.Confirmed, DateTime.UtcNow.AddMinutes(-45), DateTime.UtcNow.AddHours(-2), id: Guid.Parse("2f9a4c7e-8b3d-4e1a-5c6f-7d2b9a4e8c31")),
        new (Guid.NewGuid(), BookingStatus.Rejected, DateTime.UtcNow.AddDays(-59), DateTime.UtcNow.AddDays(-60), id: Guid.Parse("8e1d6b3f-5a9c-4d2e-7b4a-1c8f3e6d9b52")),
        new (Guid.NewGuid(), BookingStatus.Pending, null, DateTime.UtcNow.AddMinutes(-30), id: Guid.Parse("4c7f2a9e-6d1b-4e8a-3f5c-9b2e7d1a4f83"))
    ];
}
