using EventManager.Application.BackgroundServices;
using EventManager.Application.Services.Interfaces;
using EventManager.Domain.DbContexts;
using EventManager.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Channels;

namespace EventManager.Application.Services;

public static class ServiceExtensions
{
    public static IServiceCollection AddEventManager(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddSingleton<IDateTimeProvider, UtcDateTimeProvider>()

            .AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("Default")
                    ?? throw new InvalidOperationException("Connection string 'Default' not found")))
            .AddInMemoryQueue<Booking>()
            .AddSingleton<ILockProvider<Event>, SemaphoreLockProvider<Event>>()

            .AddScoped<IEventService, EventService>()
            .AddScoped<IBookingService, BookingService>()

            .AddHostedService<BookingBackgroundService>();
    }

    private static IServiceCollection AddInMemoryQueue<TEntity>(this IServiceCollection services) where TEntity : BaseEntity
    {
        return services
            .AddSingleton(Channel.CreateUnbounded<TEntity>())
            .AddSingleton<IQueue<TEntity>, InMemoryQueue<TEntity>>();
    }
}
