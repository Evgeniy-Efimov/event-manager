namespace EventManager.Application.Models.DTO.Events;

public record EventsRequestDto(
    int? Page = null,
    int? PageSize = null,
    string? Title = null,
    DateTime? From = null,
    DateTime? To = null
);
