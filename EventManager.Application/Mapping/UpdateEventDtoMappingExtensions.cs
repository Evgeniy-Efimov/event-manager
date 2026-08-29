using EventManager.Application.Models.DTO;
using EventManager.Domain.Models;

namespace EventManager.Application.Mapping;

public static class UpdateEventDtoMappingExtensions
{
    public static Event ToDomain(this UpdateEventDto eventDto)
    {
        return new Event(eventDto.Title, eventDto.Description, eventDto.StartAt, eventDto.EndAt, eventDto.Id);
    }
}
