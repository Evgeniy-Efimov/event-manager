using EventManager.Application.Constants;

namespace EventManager.Application.Extensions;

public static class PaginationExtensions
{
    public static (int Page, int PageSize, IEnumerable<TResult> Query) GetPage<TResult>(
        this IEnumerable<TResult> query, int? page, int? pageSize)
    {
        page = Math.Max(PaginationConstants.DefaultPage, page ?? PaginationConstants.DefaultPage);
        pageSize = Math.Max(1, pageSize ?? PaginationConstants.DefaultPageSize);

        return (page.Value, pageSize.Value, query
            .Skip((page.Value - 1) * pageSize.Value)
            .Take(pageSize.Value));
    }
}
