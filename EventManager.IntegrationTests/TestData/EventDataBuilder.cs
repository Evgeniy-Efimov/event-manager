using EventManager.Domain.Models;

namespace EventManager.IntegrationTests.TestData;

public static class EventDataBuilder
{
    public static Event CreateEvent(
        Guid? id = null,
        string? title = null,
        string? description = null,
        DateTime? startAt = null,
        DateTime? endAt = null,
        int? totalSeats = null,
        int? availableSeats = null)
    {
        return new Event(
            id: id,
            title: title ?? RandomDataBuilder.CreateRandomString(),
            description: description ?? RandomDataBuilder.CreateRandomString(),
            startAt: startAt ?? DateTime.UtcNow,
            endAt: endAt ?? DateTime.UtcNow.AddDays(1),
            totalSeats: totalSeats ?? RandomDataBuilder.CreateRandomInt(1, 10000),
            availableSeats: availableSeats);
    }
}
