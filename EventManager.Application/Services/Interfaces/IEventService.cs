using EventManager.Application.Models.DTO;

namespace EventManager.Application.Services.Interfaces;

public interface IEventService
{
    Task<EventDto> Get(Guid id, CancellationToken cancellationToken = default);
    Task<List<EventDto>> GetList(CancellationToken cancellationToken = default);
    Task<EventDto> Create(CreateEventDto eventDto, CancellationToken cancellationToken = default);
    Task<EventDto> Update(UpdateEventDto eventDto, CancellationToken cancellationToken = default);
    Task Delete(Guid id, CancellationToken cancellationToken = default);
}
