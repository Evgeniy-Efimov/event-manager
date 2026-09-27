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
    private IBookingService BookingService => fixture.BookingService;
    private Event[] TestEvents => EventServiceFixture.TestEvents;
    private Booking[] TestBookings => BookingServiceFixture.TestBookings;
    
    [Fact]
    public async Task Create_NewBooking_Success()
    {
        // Arrange
        var createdAt = DateTime.UtcNow;
        DateTimeProviderMock.Setup(p => p.Now).Returns(createdAt);
        var eventId = TestEvents.First().Id;

        // Act
        var result = await BookingService.Create(eventId);

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
    public async Task Create_OneEventMultipleBooking_UniqueIds()
    {
        // Arrange
        var repetitions = 5;
        var eventId = TestEvents.First().Id;

        // Act
        var results = new List<BookingDto>();

        for (var i = 0; i < repetitions; i++)
            results.Add(await BookingService.Create(eventId));

        // Assert
        var uniqueIds = results.Select(r => r.Id).Distinct().ToArray();
        Assert.Equal(repetitions, results.Count);
        Assert.Equal(repetitions, uniqueIds.Length);
    }

    [Fact]
    public async Task Create_NewBooking_EventNotFoundException()
    {
        // Arrange
        var eventId = Guid.NewGuid();

        // Act
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => BookingService.Create(eventId));

        // Assert
        Assert.NotNull(exception);
        Assert.Equal($"Event '{eventId}' not found", exception.Message);
    }

    [Fact]
    public async Task GetById_ExistedBooking_Success()
    {
        // Arrange
        var existedBooking = TestBookings.First();

        // Act
        var result = await BookingService.Get(existedBooking.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(existedBooking.Id, result.Id);
        Assert.Equal(existedBooking.Status.ToString(), result.Status);
        Assert.Equal(existedBooking.ProcessedAt, result.ProcessedAt);
        Assert.Equal(existedBooking.CreatedAt, result.CreatedAt);
        Assert.Equal(existedBooking.UpdatedAt, result.UpdatedAt);
    }

    [Fact]
    public async Task GetById_NotExistedBooking_NotFoundException()
    {
        // Arrange
        var notExistedId = Guid.NewGuid();

        // Act
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => BookingService.Get(notExistedId));

        // Assert
        Assert.NotNull(exception);
        Assert.Equal($"Booking '{notExistedId}' not found", exception.Message);
    }

    [Fact]
    public async Task GetPending_ReturnsExpected()
    {
        // Arrange
        var expectedId = Guid.Parse("c4d8f1a6-3e7b-4c2d-8a5f-1b9e6d3c7a20");

        // Act
        var result = await BookingService.GetPending();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedId, result.Id);
        Assert.Equal(BookingStatus.Pending.ToString(), result.Status);
    }

    [Fact]
    public async Task Confirm_ExistedBooking_Success()
    {
        // Arrange
        var processedAt = DateTime.UtcNow;
        DateTimeProviderMock.Setup(p => p.Now).Returns(processedAt);
        var existedBooking = TestBookings.First();

        // Act
        var result = await BookingService.Confirm(existedBooking.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(existedBooking.Id, result.Id);
        Assert.Equal(BookingStatus.Confirmed.ToString(), result.Status);
        Assert.Equal(processedAt, result.ProcessedAt);
        Assert.Equal(existedBooking.CreatedAt, result.CreatedAt);
        Assert.Equal(processedAt, result.UpdatedAt);
    }

    [Fact]
    public async Task Confirm_NotExistedBooking_NotFoundException()
    {
        // Arrange
        var notExistedId = Guid.NewGuid();

        // Act
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => BookingService.Confirm(notExistedId));

        // Assert
        Assert.NotNull(exception);
        Assert.Equal($"Booking '{notExistedId}' not found", exception.Message);
    }

    [Fact]
    public async Task Confirm_NotPending_ValidationException()
    {
        // Arrange
        var notPendingBooking = TestBookings.Single(b => b.Id == Guid.Parse("9a6c3e8b-5d1f-4b7a-2c9e-4f8b1d6a3c57"));

        // Act
        var exception = await Assert.ThrowsAsync<ValidationException>(() => BookingService.Confirm(notPendingBooking.Id));

        // Assert
        Assert.NotNull(exception);
        Assert.Equal($"Can't process booking in status '{notPendingBooking.Status}'", exception.Message);
    }

    [Fact]
    public async Task Reject_ExistedBooking_Success()
    {
        // Arrange
        var processedAt = DateTime.UtcNow;
        DateTimeProviderMock.Setup(p => p.Now).Returns(processedAt);
        var existedBooking = TestBookings.First();

        // Act
        var result = await BookingService.Reject(existedBooking.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(existedBooking.Id, result.Id);
        Assert.Equal(BookingStatus.Rejected.ToString(), result.Status);
        Assert.Equal(processedAt, result.ProcessedAt);
        Assert.Equal(existedBooking.CreatedAt, result.CreatedAt);
        Assert.Equal(processedAt, result.UpdatedAt);
    }

    [Fact]
    public async Task Reject_NotExistedBooking_NotFoundException()
    {
        // Arrange
        var notExistedId = Guid.NewGuid();

        // Act
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => BookingService.Reject(notExistedId));

        // Assert
        Assert.NotNull(exception);
        Assert.Equal($"Booking '{notExistedId}' not found", exception.Message);
    }

    [Fact]
    public async Task Reject_NotPending_ValidationException()
    {
        // Arrange
        var notPendingBooking = TestBookings.Single(b => b.Id == Guid.Parse("9a6c3e8b-5d1f-4b7a-2c9e-4f8b1d6a3c57"));

        // Act
        var exception = await Assert.ThrowsAsync<ValidationException>(() => BookingService.Reject(notPendingBooking.Id));

        // Assert
        Assert.NotNull(exception);
        Assert.Equal($"Can't process booking in status '{notPendingBooking.Status}'", exception.Message);
    }
}
