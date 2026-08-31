using EventManager.Application.Models.DTO;
using EventManager.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EventManager.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController(IEventService eventService) : ControllerBase
{
    /// <summary>
    /// Get event by ID
    /// </summary>
    /// <param name="id">Event ID</param>
    /// <param name="cancellationToken"></param>
    /// <returns>EventDto</returns>
    [HttpGet("{id}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(EventDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        return Ok(await eventService.Get(id, cancellationToken));
    }

    /// <summary>
    /// Get list with all events
    /// </summary>
    /// <returns>List of EventDto</returns>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IEnumerable<EventDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetList(CancellationToken cancellationToken)
    {
        return Ok(await eventService.GetList(cancellationToken));
    }

    /// <summary>
    /// Create event
    /// </summary>
    /// <param name="eventDto">CreateEventDto</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Created event with status 201</returns>
    [HttpPost]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateEventDto eventDto, CancellationToken cancellationToken)
    {
        var created = await eventService.Create(eventDto, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Update event by ID
    /// </summary>
    /// <param name="id">Event ID</param>
    /// <param name="eventDto">UpdateEventDto</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Empty response with status 204</returns>
    [HttpPut("{id}")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEventDto eventDto, CancellationToken cancellationToken)
    {
        if (id != eventDto.Id)
        {
            return BadRequest($"Event id mismatch, route: {id}, DTO: {eventDto.Id}");
        }

        await eventService.Update(eventDto, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Delete event by ID
    /// </summary>
    /// <param name="id">Event ID</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Empty response with status 204</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await eventService.Delete(id, cancellationToken);

        return NoContent();
    }
}
