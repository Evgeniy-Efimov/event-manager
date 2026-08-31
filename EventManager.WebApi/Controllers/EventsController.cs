using EventManager.Application.Models.DTO;
using EventManager.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EventManager.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController(IEventService eventService) : ControllerBase
{
    /// <summary>
    /// Get event by id
    /// </summary>
    /// <param name="id">Event id</param>
    /// <returns>EventDto</returns>
    [HttpGet("{id}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(EventDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id)
    {
        return Ok(eventService.Get(id));
    }

    /// <summary>
    /// Get list with all events
    /// </summary>
    /// <returns>List of EventDto</returns>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IEnumerable<EventDto>), StatusCodes.Status200OK)]
    public IActionResult GetList()
    {
        return Ok(eventService.GetList());
    }

    /// <summary>
    /// Create event
    /// </summary>
    /// <param name="eventDto">CreateEventDto</param>
    /// <returns>Created event with status 201</returns>
    [HttpPost]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] CreateEventDto eventDto)
    {
        var created = eventService.Create(eventDto);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Update event
    /// </summary>
    /// <param name="id">Event id</param>
    /// <param name="eventDto">UpdateEventDto</param>
    /// <returns>Empty response with status 204</returns>
    [HttpPut("{id}")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Update(Guid id, [FromBody] UpdateEventDto eventDto)
    {
        if (id != eventDto.Id)
        {
            return BadRequest($"Event id mismatch, route: {id}, DTO: {eventDto.Id}");
        }

        eventService.Update(eventDto);

        return NoContent();
    }

    /// <summary>
    /// Delete event by id
    /// </summary>
    /// <param name="id">Event id</param>
    /// <returns>Empty response with status 204</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id)
    {
        eventService.Delete(id);

        return NoContent();
    }
}
