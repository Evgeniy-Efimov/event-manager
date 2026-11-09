using EventManager.DataAccess.Models.Queries;
using EventManager.Domain.Models;

namespace EventManager.DataAccess.Extensions.Queries;

public static class EventQueryExtensions
{
    public static IQueryable<Event> ApplyFilters(this IQueryable<Event> query, EventsQuery eventsQuery)
    {
        if (!string.IsNullOrEmpty(eventsQuery.Title))
            query = query.Where(e => e.Title.ToLower().Contains(eventsQuery.Title.ToLower()));

        if (eventsQuery.From.HasValue)
            query = query.Where(e => e.StartAt >= eventsQuery.From.Value);

        if (eventsQuery.To.HasValue)
            query = query.Where(e => e.EndAt <= eventsQuery.To.Value);

        return query;
    }

    public static IQueryable<Event> ApplySorting(this IQueryable<Event> query)
    {
        return query.OrderByDescending(c => c.StartAt);
    }
}
