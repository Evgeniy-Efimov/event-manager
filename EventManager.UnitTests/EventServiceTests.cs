using EventManager.Application.Constants;
using EventManager.Application.Models.DTO.Events;
using EventManager.Application.Models.Exceptions;
using EventManager.Application.Services.Interfaces;
using EventManager.Domain.Models;
using EventManager.UnitTests.Fixtures;
using System.Collections;

namespace EventManager.UnitTests;

public class EventServiceTests : IClassFixture<EventServiceFixture>
{
    private readonly IEventService _eventService;
    private readonly List<Event> _testEvents;
    private const int TestEventsCount = 15;

    public EventServiceTests(EventServiceFixture fixture)
    {
        _eventService = fixture.EventService;
        _testEvents = fixture.TestEvents;

        Assert.Equal(TestEventsCount, _testEvents.Count);
    }

    [Fact]
    public async Task Create_NewEvent_Success()
    {
        // Arrange
        var newEvent = new CreateEventDto()
        {
            Title = "New event",
            Description = "About new event",
            StartAt = DateTime.Today.AddHours(9),
            EndAt = DateTime.Today.AddHours(10)
        };

        // Act
        var result = await _eventService.Create(newEvent);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(newEvent.Title, result.Title);
        Assert.Equal(newEvent.Description, result.Description);
        Assert.Equal(newEvent.StartAt, result.StartAt);
        Assert.Equal(newEvent.EndAt, result.EndAt);
    }

    [Fact]
    public async Task GetById_ExistedEvent_Success()
    {
        // Arrange
        var existedEvent = _testEvents.First();

        // Act
        var result = await _eventService.Get(existedEvent.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(existedEvent.Id, result.Id);
        Assert.Equal(existedEvent.Title, result.Title);
        Assert.Equal(existedEvent.Description, result.Description);
        Assert.Equal(existedEvent.StartAt, result.StartAt);
        Assert.Equal(existedEvent.EndAt, result.EndAt);
    }

    [Fact]
    public async Task GetById_NotExistedEvent_NotFoundException()
    {
        // Arrange
        var notExistedId = Guid.NewGuid();

        // Act
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _eventService.Get(notExistedId));

        // Assert
        Assert.NotNull(exception);
        Assert.Equal($"Event '{notExistedId}' not found", exception.Message);
    }

    [Fact]
    public async Task Update_ExistedEvent_Success()
    {
        // Arrange
        var existedEvent = _testEvents.First();
        var updatedEvent = new UpdateEventDto()
        {
            Id = existedEvent.Id,
            Title = "New title",
            Description = "New description",
            StartAt = existedEvent.StartAt.AddDays(1),
            EndAt = existedEvent.EndAt.AddDays(1)
        };

        // Act
        var result = await _eventService.Update(updatedEvent);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updatedEvent.Id, result.Id);
        Assert.Equal(updatedEvent.Title, result.Title);
        Assert.Equal(updatedEvent.Description, result.Description);
        Assert.Equal(updatedEvent.StartAt, result.StartAt);
        Assert.Equal(updatedEvent.EndAt, result.EndAt);
    }

    [Fact]
    public async Task Update_NotExistedEvent_NotFoundException()
    {
        // Arrange
        var notExistedEvent = new UpdateEventDto()
        {
            Id = Guid.NewGuid(),
            Title = "New title",
            Description = "New description",
            StartAt = DateTime.Today.AddHours(9),
            EndAt = DateTime.Today.AddHours(10)
        };

        // Act
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _eventService.Update(notExistedEvent));

        // Assert
        Assert.NotNull(exception);
        Assert.Equal($"Event '{notExistedEvent.Id}' not found", exception.Message);
    }

    [Fact]
    public async Task Delete_ExistedEvent_Success()
    {
        // Arrange
        var existedEvent = _testEvents.First();

        // Act
        await _eventService.Delete(existedEvent.Id);
    }

    [Fact]
    public async Task Delete_NotExistedEvent_NotFoundException()
    {
        // Arrange
        var notExistedId = Guid.NewGuid();

        // Act
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _eventService.Delete(notExistedId));

        // Assert
        Assert.NotNull(exception);
        Assert.Equal($"Event '{notExistedId}' not found", exception.Message);
    }

    [Theory]
    [InlineData(null, PaginationConstants.DefaultPageSize, TestEventsCount)]
    [InlineData("", PaginationConstants.DefaultPageSize, TestEventsCount)]
    [InlineData("evening", 2, 2)]
    [InlineData("Some long not existed in test data title", 0, 0)]
    public async Task GetList_FilterByName_ReturnsExpected(string? titleFilter, int expectedCount, int expectedTotalCount)
    {
        // Act
        var result = await _eventService.GetList(new EventsRequestDto() { Title = titleFilter });

        // Assert
        Assert.Equal(expectedCount, result.Results.Length);
        Assert.Equal(expectedTotalCount, result.TotalCount);

        if (!string.IsNullOrEmpty(titleFilter))
        {
            Assert.All(result.Results, r => Assert.Contains(titleFilter, r.Title, StringComparison.OrdinalIgnoreCase));
        }
    }

    public static IEnumerable<object?[]> GetList_FilterByDates_ReturnsExpected_TestData() =>
    [
        [null, null, PaginationConstants.DefaultPageSize, TestEventsCount],
        [null, DateTime.Today.AddDays(2), 4, 4],
        [DateTime.Today.AddDays(10), null, 1, 1],
        [DateTime.Today.AddDays(5), DateTime.Today.AddDays(7), 2, 2]
    ];

    [Theory]
    [MemberData(nameof(GetList_FilterByDates_ReturnsExpected_TestData))]
    public async Task GetList_FilterByDates_ReturnsExpected(
        DateTime? startAtFilter, DateTime? endAtFilter, int expectedCount, int expectedTotalCount)
    {
        // Act
        var result = await _eventService.GetList(new EventsRequestDto() { From = startAtFilter, To = endAtFilter });

        // Assert
        Assert.Equal(expectedCount, result.Results.Length);
        Assert.Equal(expectedTotalCount, result.TotalCount);
        
        if (startAtFilter.HasValue)
        {
            Assert.All(result.Results, r => Assert.True(r.StartAt >= startAtFilter.Value));
        }
        if (endAtFilter.HasValue)
        {
            Assert.All(result.Results, r => Assert.True(r.EndAt <= endAtFilter.Value));
        }
    }

    [Theory]
    [InlineData(null, null, 1, PaginationConstants.DefaultPageSize, PaginationConstants.DefaultPageSize)]
    [InlineData(-1, -1, 1, 1, 1)]
    [InlineData(0, 0, 1, 1, 1)]
    [InlineData(1, 1, 1, 1, 1)]
    [InlineData(4, 4, 4, 4, 3)]
    [InlineData(2, 15, 2, 15, 0)]
    public async Task GetList_WithPagination_ReturnsExpected(int? page, int? pageSize, int expectedPage, int expectedPageSize, int expectedCount)
    {
        // Act
        var result = await _eventService.GetList(new EventsRequestDto() { Page = page, PageSize = pageSize });

        // Assert
        Assert.Equal(expectedCount, result.Results.Length);
        Assert.Equal(expectedPage, result.Page);
        Assert.Equal(expectedPageSize, result.PageSize);
        Assert.Equal(TestEventsCount, result.TotalCount);
    }

    public class GetList_FilterWithPagination_ReturnsExpected_TestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                new EventsRequestDto(),
                PaginationConstants.DefaultPage,
                PaginationConstants.DefaultPageSize,
                PaginationConstants.DefaultPageSize,
                TestEventsCount
            };
            yield return new object[]
            {
                new EventsRequestDto()
                {
                    Title = "ING",
                    From = DateTime.Today,
                    To = DateTime.Today.AddDays(7),
                    Page = 2,
                    PageSize = 3
                },
                2, 3, 2, 5
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [Theory]
    [ClassData(typeof(GetList_FilterWithPagination_ReturnsExpected_TestData))]
    public async Task GetList_FilterWithPagination_ReturnsExpected(
        EventsRequestDto request, int expectedPage, int expectedPageSize, int expectedCount, int expectedTotalCount)
    {
        // Act
        var result = await _eventService.GetList(request);

        // Assert
        Assert.Equal(expectedPage, result.Page);
        Assert.Equal(expectedPageSize, result.PageSize);
        Assert.Equal(expectedCount, result.Results.Length);
        Assert.Equal(expectedTotalCount, result.TotalCount);

        if (!string.IsNullOrEmpty(request.Title))
        {
            Assert.All(result.Results, r => Assert.Contains(request.Title, r.Title, StringComparison.OrdinalIgnoreCase));
        }
        if (request.From.HasValue)
        {
            Assert.All(result.Results, r => Assert.True(r.StartAt >= request.From.Value));
        }
        if (request.To.HasValue)
        {
            Assert.All(result.Results, r => Assert.True(r.EndAt <= request.To.Value));
        }
    }
}
