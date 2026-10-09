using System.ComponentModel.DataAnnotations;

namespace EventManager.Application.Models.DTO.Events;

public class UpdateEventDto : BaseEventDto
{
    [Required]
    public required Guid Id { get; init; }
}
