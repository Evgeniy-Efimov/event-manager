using EventManager.Domain.Enums;
using EventManager.Domain.Models;

namespace EventManager.IntegrationTests.TestData;

public static class BookingDataBuilder
{
    public static Booking CreateBooking(
        Guid eventId,
        Guid? id = null,
        BookingStatus? status = null,
        DateTime? processedAt = null)
    {
        return new Booking(
            eventId: eventId,
            id: id,
            status: status ?? BookingStatus.Confirmed,
            processedAt: processedAt ?? DateTime.UtcNow);
    }
}
