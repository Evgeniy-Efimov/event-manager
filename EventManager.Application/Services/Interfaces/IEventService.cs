using EventManager.Application.Models.DTO;

namespace EventManager.Application.Services.Interfaces;

public interface IEventService
{
    EventDto Get(Guid id);
    List<EventDto> GetList();
    void Create(CreateEventDto eventDto);
    void Update(UpdateEventDto eventDto);
    void Delete(Guid id);
}
