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
    public async Task<EventDto> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return (await GetDomainAsync(id, cancellationToken)).ToDto();
    }

    public async Task<Event> GetDomainAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return (await repository.GetAsync(id, cancellationToken))
            ?? throw new NotFoundException($"Event '{id}' not found");
    }

    public async Task<PaginatedResultDto<EventDto>> GetListAsync(EventsRequestDto request, CancellationToken cancellationToken = default)
    {
        var query = (await repository.GetListAsync(cancellationToken)).ApplyFilters(request);
        var (page, pageSize, pageQuery) = query.ApplySorting().GetPage(request.Page, request.PageSize);

        return new PaginatedResultDto<EventDto>(query.Count(), page, pageSize, pageQuery.ToDtoArray());
    }

    public async Task<EventDto> CreateAsync(CreateEventDto eventDto, CancellationToken cancellationToken = default)
    {
        var @event = eventDto.ToDomain(dateTimeProvider.Now);
        await repository.CreateAsync(@event, cancellationToken);

        return @event.ToDto();
    }

    public async Task<EventDto> UpdateAsync(UpdateEventDto eventDto, CancellationToken cancellationToken = default)
    {
        var existedEvent = await GetDomainAsync(eventDto.Id, cancellationToken);

        return await UpdateAsync(eventDto.ToDomain(existedEvent, dateTimeProvider.Now), cancellationToken);
    }

    public async Task<EventDto> UpdateAsync(Event @event, CancellationToken cancellationToken = default)
    {
        if (!await repository.UpdateAsync(@event, cancellationToken))
            throw new NotFoundException($"Event '{@event.Id}' not found");

        return @event.ToDto();
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!await repository.DeleteAsync(id, cancellationToken))
            throw new NotFoundException($"Event '{id}' not found");
    }
}
