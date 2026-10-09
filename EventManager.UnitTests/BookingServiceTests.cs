using EventManager.Application.Models.DTO.Booking;
using EventManager.Application.Models.Exceptions;
using EventManager.Application.Services.Interfaces;
using EventManager.Domain.Enums;
using EventManager.Domain.Models;
using EventManager.UnitTests.Fixtures;
using Moq;

namespace EventManager.UnitTests;

public class BookingServiceTests(BookingServiceFixture fixture) : IClassFixture<BookingServiceFixture>
{
    private Mock<IDateTimeProvider> DateTimeProviderMock => fixture.DateTimeProviderMock;
    private IBookingService BookingService => fixture.GetBookingService();
    private IQueue<Booking> BookingQueue => fixture.GetBookingQueue();
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
        var bookingQueue = BookingQueue;
        var bookingService = fixture.GetBookingService(bookingQueue: bookingQueue);
        var bookingsCount = 5;
        var eventId = TestEvents.First().Id;
        var createdBookings = new List<BookingDto>();

        for (var i = 0; i < bookingsCount; i++)
            createdBookings.Add(await bookingService.CreateAsync(eventId));

        // Act
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var results = new List<Booking>();

        while (!cancellationTokenSource.IsCancellationRequested)
        {
            var booking = await bookingQueue.ReadAsync(cancellationTokenSource.Token);

            if (booking is null)
                break;

            results.Add(booking);
        }

        // Assert
        Assert.Equal(bookingsCount, results.Count);
        Assert.Equal(createdBookings.Select(b => b.Id).Order(), results.Select(r => r.Id).Order());
        Assert.All(results, r => Assert.Equal(BookingStatus.Pending, r.Status));
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
