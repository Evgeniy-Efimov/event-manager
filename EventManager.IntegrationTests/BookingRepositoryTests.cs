using EventManager.DataAccess.DbContexts;
using EventManager.DataAccess.Models.Exceptions;
using EventManager.DataAccess.Repositories.Interfaces;
using EventManager.IntegrationTests.Factories;
using EventManager.IntegrationTests.Fixtures;
using EventManager.IntegrationTests.TestData;
using Microsoft.EntityFrameworkCore;

namespace EventManager.IntegrationTests;

public class BookingRepositoryTests(PostgreSqlContainerFixture fixture) : PostgreSqlTestBase(fixture)
{
    private static IBookingRepository CreateBookingRepository(AppDbContext context) => ServiceFactory.CreateBookingRepository(context);

    [Fact]
    public async Task Create_NewBooking_Success()
    {
        // Arrange
        using var arrangeContext = CreateDbContext();
        var existedEvent = EventDataBuilder.CreateEvent();
        arrangeContext.Events.Add(existedEvent);
        await arrangeContext.SaveChangesAsync();
        var newBooking = BookingDataBuilder.CreateBooking(eventId: existedEvent.Id);
        using var repositoryContext = CreateDbContext();
        var repository = CreateBookingRepository(repositoryContext);

        // Act
        await repository.AddAsync(newBooking);
        await repository.SaveChangesAsync();

        // Assert
        using var assertContext = CreateDbContext();
        var createdBooking = await assertContext.Bookings.FirstOrDefaultAsync(b => b.Id == newBooking.Id);
        Assert.NotNull(createdBooking);
        Assert.Equal(newBooking.EventId, createdBooking.EventId);
    }

    [Fact]
    public async Task Create_BookingWithExistedId_ThrowsException()
    {
        // Arrange
        using var arrangeContext = CreateDbContext();
        var existedEvent = EventDataBuilder.CreateEvent();
        var existedBooking = BookingDataBuilder.CreateBooking(eventId: existedEvent.Id);
        arrangeContext.Events.Add(existedEvent);
        arrangeContext.Bookings.Add(existedBooking);
        await arrangeContext.SaveChangesAsync();
        var newBooking = BookingDataBuilder.CreateBooking(id: existedBooking.Id, eventId: existedEvent.Id);
        using var repositoryContext = CreateDbContext();
        var repository = CreateBookingRepository(repositoryContext);

        // Act
        await repository.AddAsync(newBooking);
        var exception = await Assert.ThrowsAsync<DbUpdateException>(() => repository.SaveChangesAsync());

        // Assert
        Assert.NotNull(exception);
        Assert.Contains("duplicate key value violates unique constraint \"pk_bookings\"", exception?.InnerException?.Message);
    }

    [Fact]
    public async Task Create_BookingForNotExistedEvent_ThrowsException()
    {
        // Arrange
        var newBooking = BookingDataBuilder.CreateBooking(eventId: Guid.NewGuid());
        using var repositoryContext = CreateDbContext();
        var repository = CreateBookingRepository(repositoryContext);

        // Act
        await repository.AddAsync(newBooking);
        var exception = await Assert.ThrowsAsync<DbUpdateException>(() => repository.SaveChangesAsync());

        // Assert
        Assert.NotNull(exception);
        Assert.Contains(
            "insert or update on table \"bookings\" violates foreign key constraint \"fk_bookings_events_event_id\"",
            exception?.InnerException?.Message);
    }

    [Fact]
    public async Task GetById_ExistedBooking_ReturnsBookingWithEvent()
    {
        // Arrange
        using var arrangeContext = CreateDbContext();
        var existedEvent = EventDataBuilder.CreateEvent();
        var existedBooking = BookingDataBuilder.CreateBooking(eventId: existedEvent.Id);
        arrangeContext.Events.Add(existedEvent);
        arrangeContext.Bookings.Add(existedBooking);
        await arrangeContext.SaveChangesAsync();
        using var repositoryContext = CreateDbContext();
        var repository = CreateBookingRepository(repositoryContext);

        // Act
        var booking = await repository.GetAsync(existedBooking.Id, withEvent: true);
        
        // Assert
        Assert.NotNull(booking);
        Assert.Equal(existedBooking.Id, booking.Id);
        var @event = booking.Event;
        Assert.NotNull(@event);
        Assert.Equal(existedEvent.Id, @event.Id);
    }

    [Fact]
    public async Task GetById_NotExistedBooking_ThrowsException()
    {
        // Arrange
        var notExistedId = Guid.NewGuid();
        using var repositoryContext = CreateDbContext();
        var repository = CreateBookingRepository(repositoryContext);

        // Act
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => repository.GetAsync(notExistedId));

        // Assert
        Assert.NotNull(exception);
        Assert.Contains($"Booking '{notExistedId}' not found", exception.Message);
    }

    [Fact]
    public async Task SaveChanges_UpdateExistedBooking_Success()
    {
        // Arrange
        using var arrangeContext = CreateDbContext();
        var oldEvent = EventDataBuilder.CreateEvent();
        var newEvent = EventDataBuilder.CreateEvent();
        var bookingId = Guid.NewGuid();
        arrangeContext.Events.AddRange([oldEvent, newEvent]);
        arrangeContext.Bookings.Add(BookingDataBuilder.CreateBooking(id: bookingId, eventId: oldEvent.Id));
        await arrangeContext.SaveChangesAsync();
        using var actContext = CreateDbContext();
        var repository = CreateBookingRepository(actContext);

        // Act
        var existedBooking = await actContext.Bookings.FirstAsync(b => b.Id == bookingId);
        existedBooking.EventId = newEvent.Id;
        await repository.SaveChangesAsync();

        // Assert
        using var assertContext = CreateDbContext();
        var updatedBooking = await assertContext.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId);
        Assert.NotNull(updatedBooking);
        Assert.Equal(newEvent.Id, updatedBooking.EventId);
    }
}
