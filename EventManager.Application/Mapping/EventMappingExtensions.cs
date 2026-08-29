using EventManager.Application.Models.DTO;
using EventManager.Domain.Models;

namespace EventManager.Application.Mapping;

public static class EventMappingExtensions
{
    public static EventResponseDto ToDto(this Event @event)
    {
        return new EventResponseDto(
            @event.Id,
            @event.Title,
            @event.Description,
            @event.StartAt,
            @event.EndAt);
    }

    public static List<EventResponseDto> ToDtoList(this IEnumerable<Event> @events)
    {
        return @events.Select(e => e.ToDto()).ToList();
    }
}
