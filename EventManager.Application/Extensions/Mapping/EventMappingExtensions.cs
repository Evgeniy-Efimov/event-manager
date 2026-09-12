using EventManager.Application.Models.DTO.Events;
using EventManager.Domain.Models;

namespace EventManager.Application.Extensions.Mapping;

public static class EventMappingExtensions
{
    public static EventDto ToDto(this Event @event)
    {
        return new EventDto(
            @event.Id,
            @event.Title,
            @event.Description,
            @event.StartAt,
            @event.EndAt);
    }

    public static List<EventDto> ToDtoList(this IEnumerable<Event> events)
    {
        return events.Select(e => e.ToDto()).ToList();
    }

    public static EventDto[] ToDtoArray(this IEnumerable<Event> events)
    {
        return events.Select(e => e.ToDto()).ToArray();
    }
}
