using EventManager.Application.Extensions.Mapping.Events;
using EventManager.Application.Models.DTO;
using EventManager.Application.Models.DTO.Events;
using EventManager.Application.Services.Interfaces;
using EventManager.DataAccess.Repositories.Interfaces;
using EventManager.Domain.Models;

namespace EventManager.Application.Services;

public class EventService(
    IEventRepository eventRepository,
    ILockProvider<Event> eventLock,
    IDateTimeProvider dateTimeProvider) : IEventService
{
    public async Task<EventDto> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return (await GetDomainAsync(id, cancellationToken)).ToDto();
    }

    public Task<Event> GetDomainAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return eventRepository.GetAsync(id, cancellationToken);
    }

    public async Task<PaginatedResultDto<EventDto>> GetListAsync(EventsRequestDto request, CancellationToken cancellationToken = default)
    {
        var (totalCount, page, pageSize, results) = await eventRepository.GetListAsync(request.ToQuery(), cancellationToken);

        return new PaginatedResultDto<EventDto>(totalCount, page, pageSize, results.ToDtoArray());
    }

    public async Task<EventDto> CreateAsync(CreateEventDto eventDto, CancellationToken cancellationToken = default)
    {
        var @event = eventDto.ToDomain(dateTimeProvider.Now);
        await eventRepository.AddAsync(@event, cancellationToken);
        await eventRepository.SaveChangesAsync(cancellationToken);

        return @event.ToDto();
    }

    public async Task UpdateAsync(UpdateEventDto eventDto, CancellationToken cancellationToken = default)
    {
        await using (await eventLock.LockAsync(cancellationToken))
        {
            var existedEvent = await GetDomainAsync(eventDto.Id, cancellationToken);
            existedEvent.Update(eventDto, dateTimeProvider.Now);
            await eventRepository.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await eventRepository.DeleteAsync(id, cancellationToken);
        await eventRepository.SaveChangesAsync(cancellationToken);
    }
}
