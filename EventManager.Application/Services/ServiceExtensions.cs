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
        services.AddSingleton<IDateTimeProvider, UtcDateTimeProvider>();

        services.AddSingleton<IRepository<Event>, InMemoryRepository<Event>>();
        services.AddSingleton<IRepository<Booking>, InMemoryRepository<Booking>>();
        services.AddInMemoryQueue<Booking>();

        services.AddSingleton<IEventService, EventService>();
        services.AddSingleton<IBookingService, BookingService>();

        services.AddHostedService<BookingBackgroundService>();

        return services;
    }

    private static IServiceCollection AddInMemoryQueue<TEntity>(this IServiceCollection services) where TEntity : BaseEntity
    {
        services.AddSingleton(Channel.CreateUnbounded<TEntity>());
        services.AddSingleton<IQueue<TEntity>, InMemoryQueue<TEntity>>();

        return services;
    }
}
