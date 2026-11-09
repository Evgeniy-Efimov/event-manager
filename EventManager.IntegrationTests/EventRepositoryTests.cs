using EventManager.DataAccess.Constants;
using EventManager.DataAccess.DbContexts;
using EventManager.DataAccess.Models.Exceptions;
using EventManager.DataAccess.Models.Queries;
using EventManager.DataAccess.Repositories.Interfaces;
using EventManager.Domain.Models;
using EventManager.IntegrationTests.Factories;
using EventManager.IntegrationTests.Fixtures;
using EventManager.IntegrationTests.TestData;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace EventManager.IntegrationTests;

public class EventRepositoryTests(PostgreSqlContainerFixture fixture) : PostgreSqlTestBase(fixture)
{
    private static IEventRepository CreateEventRepository(AppDbContext context) => ServiceFactory.CreateEventRepository(context);
    
    [Fact]
    public async Task Create_NewEvent_Success()
    {
        // Arrange
        var newEvent = EventDataBuilder.CreateEvent();
        using var repositoryContext = CreateDbContext();
        var repository = CreateEventRepository(repositoryContext);

        // Act
        await repository.AddAsync(newEvent);
        await repository.SaveChangesAsync();

        // Assert
        using var assertContext = CreateDbContext();
        var createdEvent = await assertContext.Events.FirstOrDefaultAsync(e => e.Id == newEvent.Id);
        Assert.NotNull(createdEvent);
        Assert.Equal(newEvent.Title, createdEvent.Title);
    }

    [Fact]
    public async Task Create_EventWithExistedId_ThrowsException()
    {
        // Arrange
        using var arrangeContext = CreateDbContext();
        var existedEvent = EventDataBuilder.CreateEvent();
        await arrangeContext.Events.AddAsync(existedEvent);
        await arrangeContext.SaveChangesAsync();
        var newEvent = EventDataBuilder.CreateEvent(id: existedEvent.Id);
        using var repositoryContext = CreateDbContext();
        var repository = CreateEventRepository(repositoryContext);

        // Act
        await repository.AddAsync(newEvent);
        var exception = await Assert.ThrowsAsync<DbUpdateException>(() => repository.SaveChangesAsync());

        // Assert
        Assert.NotNull(exception);
        Assert.Contains("duplicate key value violates unique constraint \"pk_events\"", exception?.InnerException?.Message);
    }

    [Fact]
    public async Task Create_EventWithNullTitle_ThrowsException()
    {
        // Arrange
        var newEvent = EventDataBuilder.CreateEvent();
        newEvent.Title = null!;
        using var repositoryContext = CreateDbContext();
        var repository = CreateEventRepository(repositoryContext);

        // Act
        await repository.AddAsync(newEvent);
        var exception = await Assert.ThrowsAsync<DbUpdateException>(() => repository.SaveChangesAsync());

        // Assert
        Assert.NotNull(exception);
        Assert.Contains("null value in column \"title\" of relation \"events\" violates not-null constraint", exception?.InnerException?.Message);
    }

    [Fact]
    public async Task Create_EventWithTitleExceeds100Characters_ThrowsException()
    {
        // Arrange
        var newEvent = EventDataBuilder.CreateEvent(title: RandomDataBuilder.CreateRandomString(101));
        using var repositoryContext = CreateDbContext();
        var repository = CreateEventRepository(repositoryContext);

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
        using var repositoryContext = CreateDbContext();
        var repository = CreateEventRepository(repositoryContext);

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
        using var arrangeContext = CreateDbContext();
        var existedEvent = EventDataBuilder.CreateEvent();
        await arrangeContext.Events.AddAsync(existedEvent);
        await arrangeContext.SaveChangesAsync();
        using var repositoryContext = CreateDbContext();
        var repository = CreateEventRepository(repositoryContext);

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
        using var arrangeContext = CreateDbContext();
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
        using var repositoryContext = CreateDbContext();
        var repository = CreateEventRepository(repositoryContext);

        // Act
        var @event = await repository.GetAsync(existedEvent.Id, withBookings: true);

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
        using var repositoryContext = CreateDbContext();
        var repository = CreateEventRepository(repositoryContext);

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
        using var arrangeContext = CreateDbContext();
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
        using var repositoryContext = CreateDbContext();
        var repository = CreateEventRepository(repositoryContext);

        // Act
        var (totalCount, page, pageSize, results) = await repository.GetListAsync(new EventsQuery());

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
            var now = DateTime.UtcNow;
            var existedEvents = new List<Event>();

            for (int i = 0; i < 100; i++)
            {
                existedEvents.Add(EventDataBuilder.CreateEvent(
                    id: Guid.Parse($"00000000-0000-0000-0000-{i:D12}"),
                    title: $"testEvent{i}",
                    startAt: now.AddDays(i),
                    endAt: now.AddDays(i).AddMinutes(1)));
            }

            yield return new object[]
            {
                existedEvents,
                new EventsQuery(),
                PaginationConstants.DefaultPage,
                PaginationConstants.DefaultPageSize,
                existedEvents
                    .OrderByDescending(e => e.StartAt)
                    .Take(PaginationConstants.DefaultPageSize)
                    .Select(e => e.Id)
                    .ToArray(),
                existedEvents.Count
            };

            var upperTitleFilter = "EVENT2";

            yield return new object[]
            {
                existedEvents,
                new EventsQuery(Title: upperTitleFilter),
                PaginationConstants.DefaultPage,
                PaginationConstants.DefaultPageSize,
                existedEvents
                    .Where(e => e.Title.Contains(upperTitleFilter, StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(e => e.StartAt)
                    .Take(PaginationConstants.DefaultPageSize)
                    .Select(e => e.Id)
                    .ToArray(),
                existedEvents.Count(e => e.Title.Contains(upperTitleFilter, StringComparison.OrdinalIgnoreCase))
            };
            
            var fromFilter = now.AddDays(50);

            yield return new object[]
            {
                existedEvents,
                new EventsQuery(From: fromFilter),
                PaginationConstants.DefaultPage,
                PaginationConstants.DefaultPageSize,
                existedEvents
                    .Where(e => e.StartAt >= fromFilter)
                    .OrderByDescending(e => e.StartAt)
                    .Take(PaginationConstants.DefaultPageSize)
                    .Select(e => e.Id)
                    .ToArray(),
                existedEvents.Count(e => e.StartAt >= fromFilter)
            };

            var toFilter = now.AddDays(30);

            yield return new object[]
            {
                existedEvents,
                new EventsQuery(To: toFilter),
                PaginationConstants.DefaultPage,
                PaginationConstants.DefaultPageSize,
                existedEvents
                    .Where(e => e.EndAt <= toFilter)
                    .OrderByDescending(e => e.StartAt)
                    .Take(PaginationConstants.DefaultPageSize)
                    .Select(e => e.Id)
                    .ToArray(),
                existedEvents.Count(e => e.EndAt <= toFilter)
            };

            var secondPage = 2;
            var secondPageSize = 20;

            yield return new object[]
            {
                existedEvents,
                new EventsQuery(Page: secondPage, PageSize: secondPageSize),
                secondPage,
                secondPageSize,
                existedEvents
                    .OrderByDescending(e => e.StartAt)
                    .Skip((secondPage - 1) * secondPageSize)
                    .Take(secondPageSize)
                    .Select(e => e.Id)
                    .ToArray(),
                existedEvents.Count
            };

            var lastIncompletePage = 7;
            var lastIncompletePageSize = 15;

            yield return new object[]
            {
                existedEvents,
                new EventsQuery(Page: lastIncompletePage, PageSize: lastIncompletePageSize),
                lastIncompletePage,
                lastIncompletePageSize,
                existedEvents
                    .OrderByDescending(e => e.StartAt)
                    .Skip((lastIncompletePage - 1) * lastIncompletePageSize)
                    .Take(lastIncompletePageSize)
                    .Select(e => e.Id)
                    .ToArray(),
                existedEvents.Count
            };

            var pageBeyondRange = 11;
            var pageBeyondRangeSize = 10;

            yield return new object[]
            {
                existedEvents,
                new EventsQuery(Page: pageBeyondRange, PageSize: pageBeyondRangeSize),
                pageBeyondRange,
                pageBeyondRangeSize,
                existedEvents
                    .OrderByDescending(e => e.StartAt)
                    .Skip((pageBeyondRange - 1) * pageBeyondRangeSize)
                    .Take(pageBeyondRangeSize)
                    .Select(e => e.Id)
                    .ToArray(),
                existedEvents.Count
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [Theory]
    [ClassData(typeof(GetList_FilterWithPagination_ReturnsExpected_TestData))]
    public async Task GetList_FilterWithPagination_ReturnsExpected(
        List<Event> existedEvents, EventsQuery query,
        int expectedPage, int expectedPageSize,
        Guid[] expectedIds, int expectedTotalCount)
    {
        // Arrange
        using var arrangeContext = CreateDbContext();
        await arrangeContext.Events.AddRangeAsync(existedEvents);
        await arrangeContext.SaveChangesAsync();
        using var repositoryContext = CreateDbContext();
        var repository = CreateEventRepository(repositoryContext);

        // Act
        var (totalCount, page, pageSize, results) = await repository.GetListAsync(query);

        // Assert
        var events = results?.ToArray();
        Assert.NotNull(events);
        Assert.Equal(expectedIds, events.Select(e => e.Id).ToArray());
        Assert.Equal(expectedTotalCount, totalCount);
        Assert.Equal(expectedPage, page);
        Assert.Equal(expectedPageSize, pageSize);
    }

    [Fact]
    public async Task SaveChanges_UpdateExistedEvent_Success()
    {
        // Arrange
        using var arrangeContext = CreateDbContext();
        var eventId = Guid.NewGuid();
        await arrangeContext.Events.AddAsync(EventDataBuilder.CreateEvent(id: eventId));
        await arrangeContext.SaveChangesAsync();
        using var actContext = CreateDbContext();
        var repository = CreateEventRepository(actContext);

        // Act
        var existedEvent = await actContext.Events.FirstAsync(e => e.Id == eventId);
        var newTitle = "new title";
        existedEvent.Title = newTitle;
        await repository.SaveChangesAsync();

        // Assert
        using var assertContext = CreateDbContext();
        var updatedEvent = await assertContext.Events.FirstOrDefaultAsync(e => e.Id == eventId);
        Assert.NotNull(updatedEvent);
        Assert.Equal(newTitle, updatedEvent.Title);
    }

    [Fact]
    public async Task Delete_ExistedEvent_Success()
    {
        // Arrange
        using var arrangeContext = CreateDbContext();
        var eventId = Guid.NewGuid();
        await arrangeContext.Events.AddAsync(EventDataBuilder.CreateEvent(id: eventId));
        await arrangeContext.SaveChangesAsync();
        using var repositoryContext = CreateDbContext();
        var repository = CreateEventRepository(repositoryContext);

        // Act
        await repository.DeleteAsync(eventId);
        await repository.SaveChangesAsync();

        // Assert
        using var assertContext = CreateDbContext();
        var updatedEvents = await assertContext.Events.ToListAsync();
        Assert.Empty(updatedEvents);
    }

    [Fact]
    public async Task Delete_NotExistedEvent_ThrowsException()
    {
        // Arrange
        var notExistedId = Guid.NewGuid();
        using var repositoryContext = CreateDbContext();
        var repository = CreateEventRepository(repositoryContext);

        // Act
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => repository.DeleteAsync(notExistedId));

        // Assert
        Assert.NotNull(exception);
        Assert.Contains($"Event '{notExistedId}' not found", exception.Message);
    }
}
