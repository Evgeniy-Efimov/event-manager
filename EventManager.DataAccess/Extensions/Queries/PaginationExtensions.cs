using EventManager.Domain.Constants;

namespace EventManager.DataAccess.Extensions.Queries;

public static class PaginationExtensions
{
    public static (int Page, int PageSize, IQueryable<TResult> Query) GetPage<TResult>(
        this IQueryable<TResult> query, int page, int pageSize)
    {
        page = Math.Max(PaginationConstants.DefaultPage, page);
        pageSize = Math.Min(
            PaginationConstants.MaxPageSize,
            Math.Max(PaginationConstants.MinPageSize, pageSize));

        return (page, pageSize, query
            .Skip((page - 1) * pageSize)
            .Take(pageSize));
    }
}
