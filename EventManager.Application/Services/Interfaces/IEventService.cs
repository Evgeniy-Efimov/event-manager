using EventManager.Application.Models.DTO;
using EventManager.Application.Models.DTO.Events;
using EventManager.Domain.Models;

namespace EventManager.Application.Services.Interfaces;

public interface IEventService
{
    Task<EventDto> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Event> GetDomainAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PaginatedResultDto<EventDto>> GetListAsync(EventsRequestDto request, CancellationToken cancellationToken = default);
    Task<EventDto> CreateAsync(CreateEventDto eventDto, CancellationToken cancellationToken = default);
    Task<EventDto> UpdateAsync(UpdateEventDto eventDto, CancellationToken cancellationToken = default);
    Task<EventDto> UpdateAsync(Event @event, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
