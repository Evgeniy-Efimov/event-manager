using EventManager.Domain.Models;
using EventManager.Domain.Models.Queries;

namespace EventManager.Domain.Repositories;

public interface IEventRepository
{
    Task<Event> GetAsync(Guid id, bool withBookings = false, CancellationToken cancellationToken = default);
    Task<(int TotalCount, int Page, int PageSize, IEnumerable<Event> Results)> GetListAsync(EventsQuery eventsQuery, CancellationToken cancellationToken = default);
    Task AddAsync(Event @event, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
