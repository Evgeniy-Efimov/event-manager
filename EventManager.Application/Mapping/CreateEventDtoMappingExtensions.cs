using EventManager.Application.Models.DTO;
using EventManager.Domain.Models;

namespace EventManager.Application.Mapping;

public static class CreateEventDtoMappingExtensions
{
    public static Event ToDomain(this CreateEventDto eventDto)
    {
        return new Event(eventDto.Title, eventDto.Description, eventDto.StartAt, eventDto.EndAt);
    }
}
