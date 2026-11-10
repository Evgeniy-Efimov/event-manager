using EventManager.DataAccess.DbContexts;
using EventManager.DataAccess.Repositories;
using EventManager.Domain.Repositories;

namespace EventManager.IntegrationTests.Factories;

public static class ServiceFactory
{
    public static IEventRepository CreateEventRepository(AppDbContext context) => new EventRepository(context);
    public static IBookingRepository CreateBookingRepository(AppDbContext context) => new BookingRepository(context);
}
