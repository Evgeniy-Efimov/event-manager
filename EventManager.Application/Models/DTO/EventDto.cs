namespace EventManager.Application.Models.DTO;

public record EventDto(
    Guid Id,
    string Title,
    string? Description,
    DateTime StartAt,
    DateTime EndAt
);
