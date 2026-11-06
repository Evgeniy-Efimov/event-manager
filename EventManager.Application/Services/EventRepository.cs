using EventManager.Application.Extensions;
using EventManager.Application.Extensions.Query;
using EventManager.Application.Models.DTO.Events;
using EventManager.Application.Models.Exceptions;
using EventManager.Application.Services.Interfaces;
using EventManager.Domain.DbContexts;
using EventManager.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EventManager.Application.Services;

public class EventRepository(AppDbContext context) : IEventRepository
{
    public async Task<Event> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return (await context.Events.Include(e => e.Bookings).FirstOrDefaultAsync(e => e.Id == id, cancellationToken))
            ?? throw new NotFoundException($"Event '{id}' not found");
    }

    public async Task<(int TotalCount, int Page, int PageSize, IEnumerable<Event> Results)> GetListAsync(
        EventsRequestDto request, CancellationToken cancellationToken = default)
    {
        var query = context.Events.AsNoTracking().ApplyFilters(request);
        var (page, pageSize, pageQuery) = query.ApplySorting().GetPage(request.Page, request.PageSize);

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
