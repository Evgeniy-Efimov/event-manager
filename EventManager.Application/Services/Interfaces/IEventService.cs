using EventManager.Application.Models.DTO;

namespace EventManager.Application.Services.Interfaces;

public interface IEventService
{
    EventResponseDto Get(Guid id);
    List<EventResponseDto> GetList();
    void Create(CreateEventDto eventDto);
    void Update(UpdateEventDto eventDto);
    void Delete(Guid id);
}
