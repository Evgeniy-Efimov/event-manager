using EventManager.Application.Models.Exceptions;
using EventManager.WebApi.Models;
using System.Net;
using System.Text.Json;

namespace EventManager.WebApi.Middlewares;

public class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = ex switch
        {
            NotFoundException => (int)HttpStatusCode.NotFound,
            _ => (int)HttpStatusCode.InternalServerError
        };

        var responseMessage = ex.Message;

        if (context.Response.StatusCode == (int)HttpStatusCode.InternalServerError)
        {
            _logger.LogError(ex, "Request processing error");
            responseMessage = "Internal server error occurred";
        }

        await context.Response.WriteAsync(JsonSerializer.Serialize(
            new ErrorResponse(context.Response.StatusCode, responseMessage, DateTime.UtcNow)));
    }
}
