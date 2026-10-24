using EventManager.Application.Models.DTO.Events;
using EventManager.Application.Models.Exceptions;
using EventManager.Domain.Models;

namespace EventManager.Application.Extensions.Mapping.Events;

public static class EventMappingExtensions
{
    public static EventDto ToDto(this Event @event)
    {
        return new EventDto(
            @event.Id,
            @event.Title,
            @event.Description,
            @event.StartAt,
            @event.EndAt,
            @event.TotalSeats,
            @event.AvailableSeats,
            @event.CreatedAt,
            @event.UpdatedAt);
    }

    public static EventDto[] ToDtoArray(this IEnumerable<Event> events)
    {
        return events.Select(e => e.ToDto()).ToArray();
    }

    public static Event Update(this Event @event, UpdateEventDto eventDto, DateTime updatedAt)
    {
        @event.Title = eventDto.Title;
        @event.Description = eventDto.Description;
        @event.StartAt = eventDto.StartAt ?? throw new ValidationException("StartAt required");
        @event.EndAt = eventDto.EndAt ?? throw new ValidationException("EndAt required");
        @event.UpdatedAt = updatedAt;

        return @event;
    }
}
