using EventManager.Application.Models.DTO;

namespace EventManager.Application.Services.Interfaces;

public interface IEventService
{
    EventDto Get(Guid id);
    List<EventDto> GetList();
    EventDto Create(CreateEventDto eventDto);
    EventDto Update(UpdateEventDto eventDto);
    void Delete(Guid id);
}
