namespace EventManager.Application.Models.Exceptions;

public class ValidationException(string message) : Exception(message)
{
}
