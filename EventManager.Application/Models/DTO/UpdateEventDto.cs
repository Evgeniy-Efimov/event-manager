using System.ComponentModel.DataAnnotations;

namespace EventManager.Application.Models.DTO;

public class UpdateEventDto : CreateEventDto
{
    [Required]
    public required Guid Id { get; init; }
}
