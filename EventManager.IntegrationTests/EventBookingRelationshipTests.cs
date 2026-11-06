using EventManager.IntegrationTests.Fixtures;
using EventManager.IntegrationTests.TestData;
using Microsoft.EntityFrameworkCore;

namespace EventManager.IntegrationTests;

public class EventBookingRelationshipTests(PostgreSqlContainerFixture fixture) : PostgreSqlTestBase(fixture)
{
    [Fact]
    public async Task DeleteBooking_ExistedEvent_DoesNotDeleteRelatedEvent()
    {
        // Arrange
        var arrangeContext = CreateDbContext();
        var eventId = Guid.NewGuid();
        var bookingId = Guid.NewGuid();
        arrangeContext.Events.Add(EventDataBuilder.CreateEvent(id: eventId));
        arrangeContext.Bookings.Add(BookingDataBuilder.CreateBooking(id: bookingId, eventId: eventId));
        await arrangeContext.SaveChangesAsync();
        var actContext = CreateDbContext();

        // Act
        var existedBooking = await actContext.Bookings.FirstAsync(b => b.Id == bookingId);
        actContext.Bookings.Remove(existedBooking);
        await actContext.SaveChangesAsync();

        // Assert
        var assertContext = CreateDbContext();
        var resultBookings = await assertContext.Bookings.ToListAsync();
        Assert.Empty(resultBookings);
        var resultEvent = await assertContext.Events.FirstOrDefaultAsync(e => e.Id == eventId);
        Assert.NotNull(resultEvent);
        Assert.Equal(eventId, resultEvent.Id);
    }

    [Fact]
    public async Task DeleteEvent_ExistedBookings_CascadeDeletesBookings()
    {
        // Arrange
        var arrangeContext = CreateDbContext();
        var eventId = Guid.NewGuid();
        var bookingId = Guid.NewGuid();
        arrangeContext.Events.Add(EventDataBuilder.CreateEvent(id: eventId));
        arrangeContext.Bookings.Add(BookingDataBuilder.CreateBooking(id: bookingId, eventId: eventId));
        await arrangeContext.SaveChangesAsync();
        var actContext = CreateDbContext();

        // Act
        var existedEvent = await actContext.Events.FirstAsync(e => e.Id == eventId);
        actContext.Events.Remove(existedEvent);
        await actContext.SaveChangesAsync();

        // Assert
        var assertContext = CreateDbContext();
        var resultBookings = await assertContext.Bookings.ToListAsync();
        Assert.Empty(resultBookings);
        var resultEvents = await assertContext.Events.ToListAsync();
        Assert.Empty(resultEvents);
    }
}
