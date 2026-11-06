using EventManager.Application.Services;
using EventManager.Application.Services.Interfaces;
using EventManager.Domain.DbContexts;

namespace EventManager.IntegrationTests.Factories;

public static class ServiceFactory
{
    public static IEventRepository CreateEventRepository(AppDbContext context) => new EventRepository(context);
    public static IBookingRepository CreateBookingRepository(AppDbContext context) => new BookingRepository(context);
}
