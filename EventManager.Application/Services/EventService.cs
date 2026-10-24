using EventManager.Application.Extensions;
using EventManager.Application.Extensions.Mapping.Events;
using EventManager.Application.Extensions.Query;
using EventManager.Application.Models.DTO;
using EventManager.Application.Models.DTO.Events;
using EventManager.Application.Models.Exceptions;
using EventManager.Application.Services.Interfaces;
using EventManager.Domain.DbContexts;
using EventManager.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EventManager.Application.Services;

public class EventService(
    AppDbContext context,
    ILockProvider<Event> eventLock,
    IDateTimeProvider dateTimeProvider) : IEventService
{
    public async Task<EventDto> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return (await GetDomainAsync(id, cancellationToken)).ToDto();
    }

    public async Task<Event> GetDomainAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return (await context.Events.FindAsync([id], cancellationToken))
            ?? throw new NotFoundException($"Event '{id}' not found");
    }

    public async Task<PaginatedResultDto<EventDto>> GetListAsync(EventsRequestDto request, CancellationToken cancellationToken = default)
    {
        var query = context.Events.AsNoTracking().ApplyFilters(request);
        var (page, pageSize, pageQuery) = query.ApplySorting().GetPage(request.Page, request.PageSize);

        return new PaginatedResultDto<EventDto>(query.Count(), page, pageSize, pageQuery.ToDtoArray());
    }

    public async Task<EventDto> CreateAsync(CreateEventDto eventDto, CancellationToken cancellationToken = default)
    {
        var @event = eventDto.ToDomain(dateTimeProvider.Now);
        await context.Events.AddAsync(@event, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return @event.ToDto();
    }

    public async Task UpdateAsync(UpdateEventDto eventDto, CancellationToken cancellationToken = default)
    {
        await using (await eventLock.LockAsync(cancellationToken))
        {
            var existedEvent = await GetDomainAsync(eventDto.Id, cancellationToken);
            existedEvent.Update(eventDto, dateTimeProvider.Now);
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var existedEvent = await GetDomainAsync(id, cancellationToken);
        context.Events.Remove(existedEvent);
        await context.SaveChangesAsync(cancellationToken);
    }
}
