using System.ComponentModel.DataAnnotations;

namespace EventManager.Application.Models.DTO;

public class CreateEventDto : IValidatableObject
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public required string Title { get; init; }

    [StringLength(300)]
    public string? Description { get; init; }

    [Required]
    public DateTime? StartAt { get; init; }

    [Required]
    public DateTime? EndAt { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndAt <= StartAt)
        {
            yield return new ValidationResult("EndAt must be greater than StartAt", [nameof(EndAt)]);
        }
    }
}
