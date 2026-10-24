using EventManager.Application.Services;
using EventManager.Application.Services.Interfaces;
using EventManager.Domain.DbContexts;
using EventManager.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Channels;

namespace EventManager.UnitTests.Fixtures;

public class BookingServiceFixture : EventServiceFixture
{
    public IBookingService CreateBookingService(IEventService? eventService = null, IQueue<Booking>? bookingQueue = null) => new BookingService(
        CreateTestAppDbContext(),
        bookingQueue ?? new InMemoryQueue<Booking>(Channel.CreateUnbounded<Booking>()),
        new SemaphoreLockProvider<Event>(),
        eventService ?? CreateEventService(),
        DateTimeProvider);

    public override IServiceProvider CreateServiceProvider()
    {
        var services = new ServiceCollection();
        var context = CreateTestAppDbContext();
        var eventService = CreateEventService();
        var bookingService = CreateBookingService(eventService);
        services.AddScoped<AppDbContext>(_ => context);
        services.AddScoped(_ => eventService);
        services.AddScoped(_ => bookingService);

        return services.BuildServiceProvider();
    }
}
