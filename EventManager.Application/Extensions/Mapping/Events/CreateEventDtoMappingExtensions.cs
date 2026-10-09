using EventManager.Application.Models.DTO.Events;
using EventManager.Application.Models.Exceptions;
using EventManager.Domain.Models;

namespace EventManager.Application.Extensions.Mapping.Events;

public static class CreateEventDtoMappingExtensions
{
    public static Event ToDomain(this CreateEventDto eventDto, DateTime createdAt)
    {
        return new Event(
            eventDto.Title,
            eventDto.Description,
            eventDto.StartAt ?? throw new ValidationException("StartAt required"),
            eventDto.EndAt ?? throw new ValidationException("EndAt required"),
            eventDto.TotalSeats,
            createdAt: createdAt);
    }
}
