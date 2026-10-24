using EventManager.Application.Services;
using EventManager.Application.Services.Interfaces;
using EventManager.Domain.DbContexts;
using EventManager.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Channels;

namespace EventManager.UnitTests.Fixtures;

public class BookingServiceFixture : EventServiceFixture
{
    public IBookingService CreateBookingService(
        IEventService? eventService = null,
        IQueue<Booking>? bookingQueue = null,
        AppDbContext? context = null)
    {
        return new BookingService(
            context ?? CreateAppDbContext(),
            bookingQueue ?? new InMemoryQueue<Booking>(Channel.CreateUnbounded<Booking>()),
            new SemaphoreLockProvider<Event>(),
            eventService ?? CreateEventService(),
            DateTimeProvider);
    }

    public override IServiceProvider CreateServiceProvider()
    {
        var services = new ServiceCollection();
        var bookingChannel = Channel.CreateUnbounded<Booking>();
        var bookingQueue = new InMemoryQueue<Booking>(bookingChannel);
        var context = CreateAppDbContext();
        var eventService = CreateEventService(context);
        var bookingService = CreateBookingService(eventService, bookingQueue, context);
        services.AddSingleton(_ => bookingChannel);
        services.AddSingleton(_ => bookingQueue);
        services.AddScoped(_ => context);
        services.AddScoped(_ => eventService);
        services.AddScoped(_ => bookingService);

        return services.BuildServiceProvider();
    }
}
