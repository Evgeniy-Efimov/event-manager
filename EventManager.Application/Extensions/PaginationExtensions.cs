using EventManager.Application.Constants;

namespace EventManager.Application.Extensions;

public static class PaginationExtensions
{
    public static (int Page, int PageSize, IQueryable<TResult> Query) GetPage<TResult>(
        this IQueryable<TResult> query, int? page, int? pageSize)
    {
        page = Math.Max(PaginationConstants.DefaultPage, page ?? PaginationConstants.DefaultPage);
        pageSize = Math.Min(
            PaginationConstants.MaxPageSize,
            Math.Max(PaginationConstants.MinPageSize, pageSize ?? PaginationConstants.DefaultPageSize));

        return (page.Value, pageSize.Value, query
            .Skip((page.Value - 1) * pageSize.Value)
            .Take(pageSize.Value));
    }
}
