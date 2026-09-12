namespace EventManager.Application.Models.DTO.Events;

public record EventsRequestDto(
    int? Page,
    int? PageSize,
    string? Title,
    DateTime? From,
    DateTime? To
);