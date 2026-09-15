using EventManager.Application.Models.DTO.Events;
using EventManager.Domain.Models;

namespace EventManager.Application.Extensions.Query;

public static class EventQueryExtensions
{
    public static IEnumerable<Event> ApplyFilters(this IEnumerable<Event> query, EventsRequestDto request)
    {
        if (!string.IsNullOrEmpty(request.Title))
            query = query.Where(e => e.Title.Contains(request.Title));

        if (request.From.HasValue)
            query = query.Where(e => e.StartAt >= request.From.Value);

        if (request.To.HasValue)
            query = query.Where(e => e.EndAt <= request.To.Value);

        return query;
    }

    public static IEnumerable<Event> ApplySorting(this IEnumerable<Event> query)
    {
        return query.OrderByDescending(c => c.StartAt);
    }
}
