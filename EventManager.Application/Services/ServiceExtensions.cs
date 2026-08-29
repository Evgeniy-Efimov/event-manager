using EventManager.Application.Services.Interfaces;
using EventManager.Domain.Models;
using Microsoft.Extensions.DependencyInjection;

namespace EventManager.Application.Services;

public static class ServiceExtensions
{
    public static IServiceCollection AddEventManager(this IServiceCollection services)
    {
        services.AddScoped<IRepository<Event>, InMemoryRepository<Event>>();

        return services;
    }
}
