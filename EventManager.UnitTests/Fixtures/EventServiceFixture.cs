using EventManager.Application.Services;
using EventManager.Application.Services.Interfaces;
using EventManager.Domain.DbContexts;
using EventManager.Domain.Models;
using Microsoft.Extensions.DependencyInjection;

namespace EventManager.UnitTests.Fixtures;

public class EventServiceFixture : BaseFixture
{
    public IEventService CreateEventService(AppDbContext? context = null)
    {
        return new EventService(
            context ?? CreateAppDbContext(),
            new SemaphoreLockProvider<Event>(),
            DateTimeProvider);
    }

    public override IServiceProvider CreateServiceProvider()
    {
        var services = new ServiceCollection();
        var context = CreateAppDbContext();
        var eventService = CreateEventService(context);
        services.AddScoped(_ => context);
        services.AddScoped(_ => eventService);

        return services.BuildServiceProvider();
    }
}
