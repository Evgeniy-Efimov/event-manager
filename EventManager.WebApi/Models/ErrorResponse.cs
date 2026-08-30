namespace EventManager.WebApi.Models;

public record ErrorResponse(
    int StatusCode,
    string Message,
    DateTime Timestamp
);
