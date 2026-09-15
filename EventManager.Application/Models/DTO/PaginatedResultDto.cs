namespace EventManager.Application.Models.DTO;

public record PaginatedResultDto<TResult>(
    int TotalCount,
    int Page,
    int PageSize,
    TResult[] Results
);
