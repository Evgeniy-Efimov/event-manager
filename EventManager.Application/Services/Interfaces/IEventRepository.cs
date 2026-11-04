using EventManager.Application.Models.DTO.Events;
using EventManager.Domain.Models;

namespace EventManager.Application.Services.Interfaces;

public interface IEventRepository
{
    Task<Event> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(int TotalCount, int Page, int PageSize, IEnumerable<Event> Results)> GetListAsync(EventsRequestDto request, CancellationToken cancellationToken = default);
    Task AddAsync(Event @event, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
