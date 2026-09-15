using EventManager.Application.Extensions;
using EventManager.Application.Extensions.Mapping;
using EventManager.Application.Extensions.Query;
using EventManager.Application.Models.DTO;
using EventManager.Application.Models.DTO.Events;
using EventManager.Application.Models.Exceptions;
using EventManager.Application.Services.Interfaces;
using EventManager.Domain.Models;

namespace EventManager.Application.Services;

public class EventService(IRepository<Event> repository) : IEventService
{
    public async Task<EventDto> Get(Guid id, CancellationToken cancellationToken = default)
    {
        return (await repository.Get(id, cancellationToken))?.ToDto()
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
        var @event = eventDto.ToDomain();
        await repository.Create(@event, cancellationToken);

        return @event.ToDto();
    }

    public async Task<EventDto> Update(UpdateEventDto eventDto, CancellationToken cancellationToken = default)
    {
        var @event = eventDto.ToDomain();

        if (!await repository.Update(@event, cancellationToken))
            throw new NotFoundException($"Event '{eventDto.Id}' not found");

        return @event.ToDto();
    }

    public async Task Delete(Guid id, CancellationToken cancellationToken = default)
    {
        if (!await repository.Delete(id, cancellationToken))
            throw new NotFoundException($"Event '{id}' not found");
    }
}
