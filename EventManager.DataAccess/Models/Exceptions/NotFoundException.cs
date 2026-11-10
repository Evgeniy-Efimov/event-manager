namespace EventManager.DataAccess.Models.Exceptions;

public class NotFoundException(string message) : Exception(message)
{
}
