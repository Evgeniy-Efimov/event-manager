using EventManager.Application.Constants;
using System.ComponentModel.DataAnnotations;

namespace EventManager.Application.Models.DTO.Events;

public class CreateEventDto : BaseEventDto
{
    [Required]
    [Range(1, EventConstants.MaxTotalSeats)]
    public int? TotalSeats { get; init; }
}
