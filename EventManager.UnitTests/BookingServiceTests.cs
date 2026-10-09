using EventManager.Application.Constants;
using EventManager.Application.Models.DTO.Booking;
using EventManager.Application.Models.Exceptions;
using EventManager.Application.Services.Interfaces;
using EventManager.Domain.Enums;
using EventManager.Domain.Models;
using EventManager.UnitTests.Fixtures;
using Moq;
using System.Collections.Concurrent;

namespace EventManager.UnitTests;

public class BookingServiceTests(BookingServiceFixture fixture) : IClassFixture<BookingServiceFixture>
{
    private Mock<IDateTimeProvider> DateTimeProviderMock => fixture.DateTimeProviderMock;
    private IBookingService BookingService => fixture.GetBookingService();
    private Event[] TestEvents => EventServiceFixture.TestEvents;
    private Booking[] TestBookings => BookingServiceFixture.TestBookings;
    
    [Fact]
    public async Task Create_ExistedEvent_ReturnsPending()
    {
        // Arrange
        var createdAt = DateTime.UtcNow;
        DateTimeProviderMock.Setup(p => p.Now).Returns(createdAt);
        var eventId = TestEvents.First().Id;

        // Act
        var result = await BookingService.CreateAsync(eventId);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(eventId, result.EventId);
        Assert.Equal(BookingStatus.Pending.ToString(), result.Status);
        Assert.Null(result.ProcessedAt);
        Assert.Equal(createdAt, result.CreatedAt);
        Assert.Equal(createdAt, result.UpdatedAt);
    }

    [Fact]
    public async Task Create_ExistedEvent_ReduceAvailableSeats()
    {
        // Arrange
        var @event = TestEvents.First();
        var availableSeats = @event.AvailableSeats;
        var eventService = fixture.EventService;
        var bookingService = fixture.GetBookingService(eventService);

        // Act
        _ = await bookingService.CreateAsync(@event.Id);

        // Assert
        var updatedEvent = await eventService.GetAsync(@event.Id);
        Assert.Equal(--availableSeats, updatedEvent.AvailableSeats);
    }

    [Fact]
    public async Task Create_OneEventMultipleBooking_ReturnsUniqueIds()
    {
        // Arrange
        var repetitions = 5;
        var eventId = TestEvents.First().Id;

        // Act
        var results = new List<BookingDto>();

        for (var i = 0; i < repetitions; i++)
            results.Add(await BookingService.CreateAsync(eventId));

        // Assert
        var uniqueIds = results.Select(r => r.Id).Distinct().ToArray();
        Assert.Equal(repetitions, results.Count);
        Assert.Equal(repetitions, uniqueIds.Length);
    }

    [Fact]
    public async Task Create_MultipleBooking_QueueReturnsAllPending()
    {
        // Arrange
        var bookingService = fixture.GetBookingService();
        var bookingsCount = BookingConstants.DefaultPendingBatchSize;
        var eventId = TestEvents.First().Id;
        var createdBookings = new List<BookingDto>();

        for (var i = 0; i < bookingsCount; i++)
            createdBookings.Add(await bookingService.CreateAsync(eventId));

        // Act
        var results = await bookingService.GetPendingBookingsAsync();

        // Assert
        Assert.Equal(bookingsCount, results.Count);
        Assert.Equal(createdBookings.Select(b => b.Id).Order(), results.Select(r => r.Id).Order());
        Assert.All(results, r => Assert.Equal(BookingStatus.Pending, r.Status));
    }

    [Fact]
    public async Task Create_MultipleBooking_ReduceSeatsByOneUntilError()
    {
        // Arrange
        var @event = TestEvents.First();
        var bookingsCount = @event.AvailableSeats;
        var eventService = fixture.EventService;
        var bookingService = fixture.GetBookingService(eventService);
        var availableSeatsList = new List<int>();

        // Act
        for (var i = 0; i < bookingsCount; i++)
        {
            await bookingService.CreateAsync(@event.Id);
            availableSeatsList.Add((await eventService.GetAsync(@event.Id)).AvailableSeats);
        }

        var exception = await Assert.ThrowsAsync<NoAvailableSeatsException>(() => bookingService.CreateAsync(@event.Id));

        // Assert
        Assert.NotNull(exception);
        Assert.Equal("No available seats for this event", exception.Message);
        Assert.Equal(0, availableSeatsList.Last());
        Assert.Equal(Enumerable.Range(0, bookingsCount).Reverse(), availableSeatsList);
    }

    [Fact]
    public async Task Create_NoAvailableSeats_ThrowsNoAvailableSeatsException()
    {
        // Arrange
        var eventId = Guid.Parse("b4c5d6e7-f8a9-4b0c-1d2e-3f4a5b6c7d8e");

        // Act
        var exception = await Assert.ThrowsAsync<NoAvailableSeatsException>(() => BookingService.CreateAsync(eventId));

        // Assert
        Assert.NotNull(exception);
        Assert.Equal("No available seats for this event", exception.Message);
    }

    [Fact]
    public async Task Create_EventNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var eventId = Guid.NewGuid();

        // Act
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => BookingService.CreateAsync(eventId));

        // Assert
        Assert.NotNull(exception);
        Assert.Equal($"Event '{eventId}' not found", exception.Message);
    }

    [Fact]
    public async Task Create_DeletedEvent_ThrowsNotFoundException()
    {
        // Arrange
        var eventId = TestEvents.First().Id;
        var eventService = fixture.EventService;
        var bookingService = fixture.GetBookingService(eventService);

        // Act
        await eventService.DeleteAsync(eventId);
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => bookingService.CreateAsync(eventId));

        // Assert
        Assert.NotNull(exception);
        Assert.Equal($"Event '{eventId}' not found", exception.Message);
    }

    [Fact]
    public async Task Create_ConcurrentBookings_ReturnUniqueIds()
    {
        // Arrange
        var eventId = Guid.Parse("c3d4e5f6-a7b8-4c9d-0e1f-2a3b4c5d6e7f");
        var @event = TestEvents.First(e => e.Id == eventId);
        var availableSeats = @event.AvailableSeats;
        var createdBookings = new ConcurrentBag<BookingDto>();
        var bookingService = BookingService;
        var tasks = Enumerable.Range(0, @event.AvailableSeats)
            .Select(t => Task.Run(async () => {
                createdBookings.Add(await bookingService.CreateAsync(eventId));
            }));

        // Act
        await Task.WhenAll(tasks);

        // Assert
        Assert.Equal(availableSeats, createdBookings.Count);
        var uniqueIds = createdBookings.Select(r => r.Id).Distinct().ToArray();
        Assert.Equal(createdBookings.Count, uniqueIds.Length);
    }

    [Fact]
    public async Task Create_ConcurrentBookings_HandleOverflow()
    {
        // Arrange
        var eventId = Guid.Parse("c3d4e5f6-a7b8-4c9d-0e1f-2a3b4c5d6e7f");
        var @event = TestEvents.First(e => e.Id == eventId);
        var availableSeats = @event.AvailableSeats;
        var overbookingCount = 5;
        var createdBookings = new ConcurrentBag<BookingDto>();
        var exceptions = new ConcurrentBag<Exception>();
        var bookingService = BookingService;
        var tasks = Enumerable.Range(0, @event.AvailableSeats + overbookingCount)
            .Select(t => Task.Run(async () => {
                try
                {
                    createdBookings.Add(await bookingService.CreateAsync(eventId));
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }));

        // Act
        await Task.WhenAll(tasks);

        // Assert
        Assert.Equal(availableSeats, createdBookings.Count);
        Assert.Equal(overbookingCount, exceptions.Count);
        Assert.All(exceptions, (ex) => Assert.IsType<NoAvailableSeatsException>(ex));
    }

    [Fact]
    public async Task GetById_ExistedBooking_ReturnsBooking()
    {
        // Arrange
        var existedBooking = TestBookings.First();

        // Act
        var result = await BookingService.GetAsync(existedBooking.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(existedBooking.Id, result.Id);
        Assert.Equal(existedBooking.Status.ToString(), result.Status);
        Assert.Equal(existedBooking.ProcessedAt, result.ProcessedAt);
        Assert.Equal(existedBooking.CreatedAt, result.CreatedAt);
        Assert.Equal(existedBooking.UpdatedAt, result.UpdatedAt);
    }

    [Fact]
    public async Task GetById_NotExistedBooking_ThrowsNotFoundException()
    {
        // Arrange
        var notExistedId = Guid.NewGuid();

        // Act
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => BookingService.GetAsync(notExistedId));

        // Assert
        Assert.NotNull(exception);
        Assert.Equal($"Booking '{notExistedId}' not found", exception.Message);
    }

    [Fact]
    public async Task GetById_ConfirmedBooking_ReturnsConfirmed()
    {
        // Arrange
        var pedningBookingId = Guid.Parse("3f8a1c2e-9b4d-4e6a-8f1b-2c7d5e9a0b3f");
        var bookingService = BookingService;

        // Act
        var pedningBooking = await bookingService.GetAsync(pedningBookingId);
        await bookingService.ConfirmAsync(pedningBookingId);
        var confirmedBooking = await bookingService.GetAsync(pedningBookingId);

        // Assert
        Assert.NotNull(pedningBooking);
        Assert.Equal(BookingStatus.Pending.ToString(), pedningBooking.Status);
        Assert.NotNull(confirmedBooking);
        Assert.Equal(BookingStatus.Confirmed.ToString(), confirmedBooking.Status);
    }

    [Fact]
    public async Task GetById_RejectedBooking_ReturnsRejected()
    {
        // Arrange
        var pedningBookingId = Guid.Parse("3f8a1c2e-9b4d-4e6a-8f1b-2c7d5e9a0b3f");
        var bookingService = BookingService;

        // Act
        var pedningBooking = await bookingService.GetAsync(pedningBookingId);
        await bookingService.RejectAsync(pedningBookingId);
        var rejectedBooking = await bookingService.GetAsync(pedningBookingId);

        // Assert
        Assert.NotNull(pedningBooking);
        Assert.Equal(BookingStatus.Pending.ToString(), pedningBooking.Status);
        Assert.NotNull(rejectedBooking);
        Assert.Equal(BookingStatus.Rejected.ToString(), rejectedBooking.Status);
    }

    [Fact]
    public async Task Confirm_ExistedPendingBooking_ReturnsConfirmed()
    {
        // Arrange
        var processedAt = DateTime.UtcNow;
        DateTimeProviderMock.Setup(p => p.Now).Returns(processedAt);
        var existedBooking = TestBookings.First();

        // Act
        var result = await BookingService.ConfirmAsync(existedBooking.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(existedBooking.Id, result.Id);
        Assert.Equal(BookingStatus.Confirmed.ToString(), result.Status);
        Assert.Equal(processedAt, result.ProcessedAt);
        Assert.Equal(existedBooking.CreatedAt, result.CreatedAt);
        Assert.Equal(processedAt, result.UpdatedAt);
    }

    [Fact]
    public async Task Confirm_NotExistedBooking_ThrowsNotFoundException()
    {
        // Arrange
        var notExistedId = Guid.NewGuid();

        // Act
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => BookingService.ConfirmAsync(notExistedId));

        // Assert
        Assert.NotNull(exception);
        Assert.Equal($"Booking '{notExistedId}' not found", exception.Message);
    }

    [Fact]
    public async Task Confirm_NonPendingBooking_ThrowsValidationException()
    {
        // Arrange
        var notPendingBooking = TestBookings.Single(b => b.Id == Guid.Parse("9a6c3e8b-5d1f-4b7a-2c9e-4f8b1d6a3c57"));

        // Act
        var exception = await Assert.ThrowsAsync<ValidationException>(() => BookingService.ConfirmAsync(notPendingBooking.Id));

        // Assert
        Assert.NotNull(exception);
        Assert.Equal($"Can't process booking in status '{notPendingBooking.Status}'", exception.Message);
    }

    [Fact]
    public async Task Reject_ExistedPendingBooking_IncreaseAvailableSeats()
    {
        // Arrange
        var eventId = Guid.Parse("d0e1f2a3-b4c5-4d6e-7f8a-9b0c1d2e3f4a");
        var bookingId = Guid.Parse("4c7f2a9e-6d1b-4e8a-3f5c-9b2e7d1a4f83");
        var @event = TestEvents.First(e => e.Id == eventId);
        var availableSeats = @event.AvailableSeats;
        var eventService = fixture.EventService;
        var bookingService = fixture.GetBookingService(eventService);

        // Act
        var result = await bookingService.RejectAsync(bookingId);

        // Assert
        var updatedEvent = await eventService.GetAsync(eventId);
        Assert.Equal(++availableSeats, updatedEvent.AvailableSeats);
    }

    [Fact]
    public async Task Reject_NoAvailableSeats_AllowsCreateBooking()
    {
        // Arrange
        var eventId = Guid.Parse("b4c5d6e7-f8a9-4b0c-1d2e-3f4a5b6c7d8e");
        var bookingId = Guid.Parse("c4d8f1a6-3e7b-4c2d-8a5f-1b9e6d3c7a20");
        var eventService = fixture.EventService;
        var bookingService = fixture.GetBookingService(eventService);

        // Act
        _ = await bookingService.RejectAsync(bookingId);
        var newBooking = await bookingService.CreateAsync(eventId);

        // Assert
        var updatedEvent = await eventService.GetAsync(eventId);
        Assert.Equal(0, updatedEvent.AvailableSeats);
        Assert.NotEqual(newBooking.Id, bookingId);
    }

    [Fact]
    public async Task Reject_ExistedPendingBooking_ReturnsRejected()
    {
        // Arrange
        var processedAt = DateTime.UtcNow;
        DateTimeProviderMock.Setup(p => p.Now).Returns(processedAt);
        var existedBooking = TestBookings.First();

        // Act
        var result = await BookingService.RejectAsync(existedBooking.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(existedBooking.Id, result.Id);
        Assert.Equal(BookingStatus.Rejected.ToString(), result.Status);
        Assert.Equal(processedAt, result.ProcessedAt);
        Assert.Equal(existedBooking.CreatedAt, result.CreatedAt);
        Assert.Equal(processedAt, result.UpdatedAt);
    }

    [Fact]
    public async Task Reject_NotExistedBooking_ThrowsNotFoundException()
    {
        // Arrange
        var notExistedId = Guid.NewGuid();

        // Act
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => BookingService.RejectAsync(notExistedId));

        // Assert
        Assert.NotNull(exception);
        Assert.Equal($"Booking '{notExistedId}' not found", exception.Message);
    }

    [Fact]
    public async Task Reject_NotPendingBooking_ThrowsValidationException()
    {
        // Arrange
        var notPendingBooking = TestBookings.Single(b => b.Id == Guid.Parse("9a6c3e8b-5d1f-4b7a-2c9e-4f8b1d6a3c57"));

        // Act
        var exception = await Assert.ThrowsAsync<ValidationException>(() => BookingService.RejectAsync(notPendingBooking.Id));

        // Assert
        Assert.NotNull(exception);
        Assert.Equal($"Can't process booking in status '{notPendingBooking.Status}'", exception.Message);
    }
}
