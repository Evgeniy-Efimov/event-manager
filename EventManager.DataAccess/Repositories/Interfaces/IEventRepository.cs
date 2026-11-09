using EventManager.DataAccess.Models.Queries;
using EventManager.Domain.Models;

namespace EventManager.DataAccess.Repositories.Interfaces;

public interface IEventRepository
{
    Task<Event> GetAsync(Guid id, bool withBookings = false, CancellationToken cancellationToken = default);
    Task<(int TotalCount, int Page, int PageSize, IEnumerable<Event> Results)> GetListAsync(EventsQuery eventsQuery, CancellationToken cancellationToken = default);
    Task AddAsync(Event @event, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
