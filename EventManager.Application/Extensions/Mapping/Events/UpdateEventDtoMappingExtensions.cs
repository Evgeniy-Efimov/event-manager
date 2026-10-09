using EventManager.Application.Models.DTO.Events;
using EventManager.Application.Models.Exceptions;
using EventManager.Domain.Models;

namespace EventManager.Application.Extensions.Mapping.Events;

public static class UpdateEventDtoMappingExtensions
{
    public static Event ToDomain(this UpdateEventDto eventDto, Event @event, DateTime updatedAt)
    {
        return new Event(
            eventDto.Title,
            eventDto.Description,
            eventDto.StartAt ?? throw new ValidationException("StartAt required"),
            eventDto.EndAt ?? throw new ValidationException("EndAt required"),
            @event.TotalSeats,
            @event.AvailableSeats,
            @event.CreatedAt,
            updatedAt,
            @event.Id);
    }
}
