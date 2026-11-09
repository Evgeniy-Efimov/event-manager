using EventManager.DataAccess.DbContexts;
using EventManager.DataAccess.Extensions.Queries;
using EventManager.DataAccess.Models.Exceptions;
using EventManager.DataAccess.Models.Queries;
using EventManager.DataAccess.Repositories.Interfaces;
using EventManager.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EventManager.DataAccess.Repositories;

public class EventRepository(AppDbContext context) : IEventRepository
{
    public async Task<Event> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return (await context.Events.Include(e => e.Bookings).FirstOrDefaultAsync(e => e.Id == id, cancellationToken))
            ?? throw new NotFoundException($"Event '{id}' not found");
    }

    public async Task<(int TotalCount, int Page, int PageSize, IEnumerable<Event> Results)> GetListAsync(
        EventsQuery eventsQuery, CancellationToken cancellationToken = default)
    {
        var query = context.Events.AsNoTracking().ApplyFilters(eventsQuery);
        var (page, pageSize, pageQuery) = query.ApplySorting().GetPage(eventsQuery.Page, eventsQuery.PageSize);

        return (await query.CountAsync(cancellationToken), page, pageSize, await pageQuery.ToListAsync(cancellationToken));
    }

    public Task AddAsync(Event @event, CancellationToken cancellationToken = default)
    {
        return context.Events.AddAsync(@event, cancellationToken).AsTask();
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var existedEvent = await GetAsync(id, cancellationToken);
        context.Events.Remove(existedEvent);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return context.SaveChangesAsync(cancellationToken);
    }
}
