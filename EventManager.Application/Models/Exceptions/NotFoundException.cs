namespace EventManager.Application.Models.Exceptions;

public class NotFoundException(string message) : Exception(message)
{
}
