using EventManager.Application.Constants;
using EventManager.Application.Models.DTO.Events;
using EventManager.Application.Models.Exceptions;
using EventManager.Application.Services.Interfaces;
using EventManager.Domain.DbContexts;
using EventManager.Domain.Models;
using EventManager.IntegrationTests.Factories;
using EventManager.IntegrationTests.Fixtures;
using EventManager.IntegrationTests.TestData;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace EventManager.IntegrationTests;

public class EventRepositoryTests(PostgreSqlContainerFixture fixture) : PostgreSqlTestBase(fixture)
{
    private IEventRepository CreateEventRepository(AppDbContext? context = null)
        => ServiceFactory.CreateEventRepository(context ?? Fixture.CreateContext());
    
    [Fact]
    public async Task Create_NewEvent_Success()
    {
        // Arrange
        var newEvent = EventDataBuilder.CreateEvent();
        var repository = CreateEventRepository();

        // Act
        await repository.AddAsync(newEvent);
        await repository.SaveChangesAsync();

        // Assert
        var assertContext = CreateDbContext();
        var createdEvent = await assertContext.Events.FirstOrDefaultAsync(e => e.Id == newEvent.Id);
        Assert.NotNull(createdEvent);
        Assert.Equal(newEvent.Title, createdEvent.Title);
    }

    [Fact]
    public async Task Create_EventWithExistedId_ThrowsException()
    {
        // Arrange
        var arrangeContext = CreateDbContext();
        var existedEvent = EventDataBuilder.CreateEvent();
        await arrangeContext.Events.AddAsync(existedEvent);
        await arrangeContext.SaveChangesAsync();
        var newEvent = EventDataBuilder.CreateEvent(id: existedEvent.Id);
        var repository = CreateEventRepository();

        // Act
        await repository.AddAsync(newEvent);
        var exception = await Assert.ThrowsAsync<DbUpdateException>(() => repository.SaveChangesAsync());

        // Assert
        Assert.NotNull(exception);
        Assert.Contains("duplicate key value violates unique constraint \"PK_Events\"", exception?.InnerException?.Message);
    }

    [Fact]
    public async Task Create_EventWithNullTitle_ThrowsException()
    {
        // Arrange
        var newEvent = EventDataBuilder.CreateEvent();
        newEvent.Title = null!;
        var repository = CreateEventRepository();

        // Act
        await repository.AddAsync(newEvent);
        var exception = await Assert.ThrowsAsync<DbUpdateException>(() => repository.SaveChangesAsync());

        // Assert
        Assert.NotNull(exception);
        Assert.Contains("null value in column \"Title\" of relation \"Events\" violates not-null constraint", exception?.InnerException?.Message);
    }

    [Fact]
    public async Task Create_EventWithTitleExceeds100Characters_ThrowsException()
    {
        // Arrange
        var newEvent = EventDataBuilder.CreateEvent(title: RandomDataBuilder.CreateRandomString(101));
        var repository = CreateEventRepository();

        // Act
        await repository.AddAsync(newEvent);
        var exception = await Assert.ThrowsAsync<DbUpdateException>(() => repository.SaveChangesAsync());

        // Assert
        Assert.NotNull(exception);
        Assert.Contains("value too long for type character varying(100)", exception?.InnerException?.Message);
    }

    [Fact]
    public async Task Create_EventWithDescriptionExceeds300Characters_ThrowsException()
    {
        // Arrange
        var newEvent = EventDataBuilder.CreateEvent(description: RandomDataBuilder.CreateRandomString(301));
        var repository = CreateEventRepository();

        // Act
        await repository.AddAsync(newEvent);
        var exception = await Assert.ThrowsAsync<DbUpdateException>(() => repository.SaveChangesAsync());

        // Assert
        Assert.NotNull(exception);
        Assert.Contains("value too long for type character varying(300)", exception?.InnerException?.Message);
    }

    [Fact]
    public async Task GetById_ExistedEvent_ReturnsExpected()
    {
        // Arrange
        var arrangeContext = CreateDbContext();
        var existedEvent = EventDataBuilder.CreateEvent();
        await arrangeContext.Events.AddAsync(existedEvent);
        await arrangeContext.SaveChangesAsync();
        var repository = CreateEventRepository();

        // Act
        var @event = await repository.GetAsync(existedEvent.Id);

        // Assert
        Assert.NotNull(@event);
        Assert.Equal(existedEvent.Id, @event.Id);
        Assert.Equal(existedEvent.Title, @event.Title);
    }

    [Fact]
    public async Task GetById_ExistedEventWithBookings_ReturnsBookingReferences()
    {
        // Arrange
        var arrangeContext = CreateDbContext();
        var existedEvent = EventDataBuilder.CreateEvent();
        var existedBookings = new List<Booking>()
        {
            BookingDataBuilder.CreateBooking(eventId: existedEvent.Id),
            BookingDataBuilder.CreateBooking(eventId: existedEvent.Id),
            BookingDataBuilder.CreateBooking(eventId: existedEvent.Id)
        };
        await arrangeContext.Events.AddAsync(existedEvent);
        await arrangeContext.Bookings.AddRangeAsync(existedBookings);
        await arrangeContext.SaveChangesAsync();
        var repository = CreateEventRepository();

        // Act
        var @event = await repository.GetAsync(existedEvent.Id);

        // Assert
        Assert.NotNull(@event);
        Assert.Equal(existedEvent.Id, @event.Id);
        Assert.Equal(existedEvent.Title, @event.Title);
        Assert.Equal(existedBookings.Select(b => b.Id).Order(), @event.Bookings.Select(b => b.Id).Order());
    }

    [Fact]
    public async Task GetById_NotExistedEvent_ThrowsException()
    {
        // Arrange
        var notExistedId = Guid.NewGuid();
        var repository = CreateEventRepository();

        // Act
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => repository.GetAsync(notExistedId));

        // Assert
        Assert.NotNull(exception);
        Assert.Contains($"Event '{notExistedId}' not found", exception.Message);
    }

    [Fact]
    public async Task GetList_WithoutFilter_ReturnsAllEvents()
    {
        // Arrange
        var arrangeContext = CreateDbContext();
        var existedEvents = new List<Event>()
        {
            EventDataBuilder.CreateEvent(),
            EventDataBuilder.CreateEvent(),
            EventDataBuilder.CreateEvent(),
            EventDataBuilder.CreateEvent(),
            EventDataBuilder.CreateEvent()
        };
        await arrangeContext.Events.AddRangeAsync(existedEvents);
        await arrangeContext.SaveChangesAsync();
        var repository = CreateEventRepository();

        // Act
        var (totalCount, page, pageSize, results) = await repository.GetListAsync(new EventsRequestDto());

        // Assert
        var events = results?.ToArray();
        Assert.NotNull(events);
        Assert.Equal(existedEvents.Count, events.Length);
        Assert.Equal(existedEvents.Select(e => e.Id).Order(), events.Select(e => e.Id).Order());
        Assert.Equal(existedEvents.Count, totalCount);
        Assert.Equal(PaginationConstants.DefaultPage, page);
        Assert.Equal(PaginationConstants.DefaultPageSize, pageSize);
    }

    public class GetList_FilterWithPagination_ReturnsExpected_TestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var titlePrefix = "testEvent";
            var titleFilter = $"{titlePrefix}1";
            var now = DateTime.UtcNow;
            var fromFilter = now.AddDays(10).Date;
            var toFilter = now.AddDays(20).Date;
            var page = 2;
            var pageSize = 3;
            var existedEvents = new List<Event>();
            var filteredEvents = existedEvents
                .Where(e => e.Title.Contains(titleFilter, StringComparison.OrdinalIgnoreCase))
                .Where(e => e.StartAt >= fromFilter)
                .Where(e => e.EndAt <= toFilter);

            for (int i = 0; i < 100; i++)
            {
                existedEvents.Add(EventDataBuilder.CreateEvent(
                    title: $"{titlePrefix}{i}",
                    startAt: now.AddDays(i),
                    endAt: now.AddDays(i).AddMinutes(1)));
            }

            yield return new object[]
            {
                existedEvents,
                new EventsRequestDto(),
                PaginationConstants.DefaultPage,
                PaginationConstants.DefaultPageSize,
                existedEvents
                    .OrderByDescending(e => e.StartAt)
                    .Take(PaginationConstants.DefaultPageSize)
                    .Select(e => e.Id)
                    .ToArray(),
                existedEvents.Count
            };

            yield return new object[]
            {
                existedEvents,
                new EventsRequestDto()
                {
                    Title = titleFilter,
                    From = fromFilter,
                    To = toFilter,
                    Page = page,
                    PageSize = pageSize
                },
                page,
                pageSize,
                filteredEvents
                    .OrderByDescending(e => e.StartAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(e => e.Id)
                    .ToArray(),
                filteredEvents.Count()
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [Theory]
    [ClassData(typeof(GetList_FilterWithPagination_ReturnsExpected_TestData))]
    public async Task GetList_FilterWithPagination_ReturnsExpected(
        List<Event> existedEvents, EventsRequestDto request,
        int expectedPage, int expectedPageSize,
        Guid[] expectedIds, int expectedTotalCount)
    {
        // Arrange
        var arrangeContext = CreateDbContext();
        await arrangeContext.Events.AddRangeAsync(existedEvents);
        await arrangeContext.SaveChangesAsync();
        var repository = CreateEventRepository();

        // Act
        var (totalCount, page, pageSize, results) = await repository.GetListAsync(request);

        // Assert
        var events = results?.ToArray();
        Assert.NotNull(events);
        Assert.Equal(expectedIds.Length, events.Length);
        Assert.Equal(expectedIds, events.Select(e => e.Id).ToArray());
        Assert.Equal(expectedTotalCount, totalCount);
        Assert.Equal(expectedPage, page);
        Assert.Equal(expectedPageSize, pageSize);
    }

    [Fact]
    public async Task SaveChanges_UpdateExistedEvent_Success()
    {
        // Arrange
        var arrangeContext = CreateDbContext();
        var eventId = Guid.NewGuid();
        await arrangeContext.Events.AddAsync(EventDataBuilder.CreateEvent(id: eventId));
        await arrangeContext.SaveChangesAsync();
        var actContext = CreateDbContext();
        var repository = CreateEventRepository(actContext);

        // Act
        var existedEvent = await actContext.Events.FirstAsync(e => e.Id == eventId);
        var newTitle = "new title";
        existedEvent.Title = newTitle;
        await repository.SaveChangesAsync();

        // Assert
        var assertContext = CreateDbContext();
        var updatedEvent = await assertContext.Events.FirstOrDefaultAsync(e => e.Id == eventId);
        Assert.NotNull(updatedEvent);
        Assert.Equal(newTitle, updatedEvent.Title);
    }

    [Fact]
    public async Task Delete_ExistedEvent_Success()
    {
        // Arrange
        var arrangeContext = CreateDbContext();
        var eventId = Guid.NewGuid();
        await arrangeContext.Events.AddAsync(EventDataBuilder.CreateEvent(id: eventId));
        await arrangeContext.SaveChangesAsync();
        var repository = CreateEventRepository();

        // Act
        await repository.DeleteAsync(eventId);
        await repository.SaveChangesAsync();

        // Assert
        var assertContext = CreateDbContext();
        var updatedEvents = await assertContext.Events.ToListAsync();
        Assert.Empty(updatedEvents);
    }

    [Fact]
    public async Task Delete_NotExistedEvent_ThrowsException()
    {
        // Arrange
        var notExistedId = Guid.NewGuid();
        var repository = CreateEventRepository();

        // Act
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => repository.DeleteAsync(notExistedId));

        // Assert
        Assert.NotNull(exception);
        Assert.Contains($"Event '{notExistedId}' not found", exception.Message);
    }
}
