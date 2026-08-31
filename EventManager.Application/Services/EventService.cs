using EventManager.Application.Mapping;
using EventManager.Application.Models.DTO;
using EventManager.Application.Models.Exceptions;
using EventManager.Application.Services.Interfaces;
using EventManager.Domain.Models;

namespace EventManager.Application.Services;

public class EventService(IRepository<Event> repository) : IEventService
{
    public EventDto Get(Guid id)
    {
        return repository.Get(id)?.ToDto() ?? throw new NotFoundException($"Event '{id}' not found");
    }

    public List<EventDto> GetList()
    {
        return repository.GetList().ToDtoList();
    }

    public EventDto Create(CreateEventDto eventDto)
    {
        var @event = eventDto.ToDomain();
        repository.Create(@event);

        return @event.ToDto();
    }

    public EventDto Update(UpdateEventDto eventDto)
    {
        var @event = eventDto.ToDomain();

        if (!repository.Update(@event))
            throw new NotFoundException($"Event '{eventDto.Id}' not found");

        return @event.ToDto();
    }

    public void Delete(Guid id)
    {
        if (!repository.Delete(id))
            throw new NotFoundException($"Event '{id}' not found");
    }
}
