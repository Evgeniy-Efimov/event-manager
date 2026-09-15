namespace EventManager.Application.Models.DTO.Events;

public record EventDto(
    Guid Id,
    string Title,
    string? Description,
    DateTime StartAt,
    DateTime EndAt
);
