using EventManager.WebApi.Middlewares;

namespace EventManager.WebApi.Extensions;

public static class MiddlewaresExtensions
{
    public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ErrorHandlingMiddleware>();
    }
}
