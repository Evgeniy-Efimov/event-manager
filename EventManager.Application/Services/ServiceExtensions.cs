using EventManager.Application.Services.Interfaces;
using EventManager.Domain.Models;
using Microsoft.Extensions.DependencyInjection;

namespace EventManager.Application.Services;

public static class ServiceExtensions
{
    public static IServiceCollection AddEventManager(this IServiceCollection services)
    {
        services.AddSingleton<IRepository<Event>, InMemoryRepository<Event>>();
        services.AddSingleton<IEventService, EventService>();

        return services;
    }
}
