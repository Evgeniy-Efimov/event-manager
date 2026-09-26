using EventManager.Application.Models.DTO.Booking;
using EventManager.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EventManager.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController(IBookingService bookingService) : ControllerBase
{
    /// <summary>
    /// Get booking by ID
    /// </summary>
    /// <param name="id">Booking ID</param>
    /// <param name="cancellationToken"></param>
    /// <returns>BookingDto</returns>
    [HttpGet("{id}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(BookingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        return Ok(await bookingService.Get(id, cancellationToken));
    }
}
