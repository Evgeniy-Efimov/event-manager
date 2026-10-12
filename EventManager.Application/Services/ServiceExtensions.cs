using EventManager.Application.BackgroundServices;
using EventManager.Application.Services.Interfaces;
using EventManager.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Channels;

namespace EventManager.Application.Services;

public static class ServiceExtensions
{
    public static IServiceCollection AddEventManager(this IServiceCollection services)
    {
        return services
            .AddSingleton<IDateTimeProvider, UtcDateTimeProvider>()

            .AddSingleton<IRepository<Event>, InMemoryRepository<Event>>()
            .AddSingleton<IRepository<Booking>, InMemoryRepository<Booking>>()
            .AddInMemoryQueue<Booking>()
            .AddSingleton<ILockProvider<Event>, SemaphoreLockProvider<Event>>()

            .AddSingleton<IEventService, EventService>()
            .AddSingleton<IBookingService, BookingService>()

            .AddHostedService<BookingBackgroundService>();
    }

    private static IServiceCollection AddInMemoryQueue<TEntity>(this IServiceCollection services) where TEntity : BaseEntity
    {
        return services
            .AddSingleton(Channel.CreateUnbounded<TEntity>())
            .AddSingleton<IQueue<TEntity>, InMemoryQueue<TEntity>>();
    }
}
