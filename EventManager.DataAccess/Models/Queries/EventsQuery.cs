using EventManager.DataAccess.Constants;

namespace EventManager.DataAccess.Models.Queries;

public record EventsQuery(
    int Page = PaginationConstants.DefaultPage,
    int PageSize = PaginationConstants.DefaultPageSize,
    string? Title = null,
    DateTime? From = null,
    DateTime? To = null
);
