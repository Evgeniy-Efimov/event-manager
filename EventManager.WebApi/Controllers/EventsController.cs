using EventManager.Application.Models.DTO;
using EventManager.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EventManager.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class EventsController(IEventService eventService) : ControllerBase
{
    [HttpGet("{id}")]
    public IActionResult GetById(Guid id)
    {
        return Ok(eventService.Get(id));
    }

    [HttpGet]
    public IActionResult GetList()
    {
        return Ok(eventService.GetList());
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateEventDto eventDto)
    {
        eventService.Create(eventDto);
        return Created();
    }

    [HttpPut]
    public IActionResult Update([FromBody] UpdateEventDto eventDto)
    {
        eventService.Update(eventDto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(Guid id)
    {
        eventService.Delete(id);
        return NoContent();
    }
}
