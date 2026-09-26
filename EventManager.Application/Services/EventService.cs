using EventManager.Application.Extensions;
using EventManager.Application.Extensions.Mapping.Events;
using EventManager.Application.Extensions.Query;
using EventManager.Application.Models.DTO;
using EventManager.Application.Models.DTO.Events;
using EventManager.Application.Models.Exceptions;
using EventManager.Application.Services.Interfaces;
using EventManager.Domain.Models;

namespace EventManager.Application.Services;

public class EventService(IRepository<Event> repository, IDateTimeProvider dateTimeProvider) : IEventService
{
    public async Task<EventDto> Get(Guid id, CancellationToken cancellationToken = default)
    {
        return (await GetDomain(id, cancellationToken)).ToDto();
    }

    public async Task<Event> GetDomain(Guid id, CancellationToken cancellationToken = default)
    {
        return (await repository.Get(id, cancellationToken))
            ?? throw new NotFoundException($"Event '{id}' not found");
    }

    public async Task<PaginatedResultDto<EventDto>> GetList(EventsRequestDto request, CancellationToken cancellationToken = default)
    {
        var query = (await repository.GetList(cancellationToken)).ApplyFilters(request);
        var (page, pageSize, pageQuery) = query.ApplySorting().GetPage(request.Page, request.PageSize);

        return new PaginatedResultDto<EventDto>(query.Count(), page, pageSize, pageQuery.ToDtoArray());
    }

    public async Task<EventDto> Create(CreateEventDto eventDto, CancellationToken cancellationToken = default)
    {
        var @event = eventDto.ToDomain(dateTimeProvider.Now);
        await repository.Create(@event, cancellationToken);

        return @event.ToDto();
    }

    public async Task<EventDto> Update(UpdateEventDto eventDto, CancellationToken cancellationToken = default)
    {
        var @event = await GetDomain(eventDto.Id, cancellationToken);
        var updatedEvent = eventDto.ApplyToDomain(@event, dateTimeProvider.Now);

        if (!await repository.Update(updatedEvent, cancellationToken))
            throw new InvalidOperationException($"Failed to update event '{eventDto.Id}'");

        return updatedEvent.ToDto();
    }

    public async Task Delete(Guid id, CancellationToken cancellationToken = default)
    {
        if (!await repository.Delete(id, cancellationToken))
            throw new NotFoundException($"Event '{id}' not found");
    }
}
