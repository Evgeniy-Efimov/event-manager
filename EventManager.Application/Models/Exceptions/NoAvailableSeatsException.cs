namespace EventManager.Application.Models.Exceptions;

public class NoAvailableSeatsException(Guid eventId) : Exception("No available seats")
{
    public string Details { get; } = $"No available seats for event '{eventId}'";
}
