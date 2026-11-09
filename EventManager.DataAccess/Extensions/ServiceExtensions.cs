using EventManager.DataAccess.DbContexts;
using EventManager.DataAccess.Repositories;
using EventManager.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventManager.DataAccess.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddDbContext<AppDbContext>(options => options
                .ConfigureDatabase(configuration.GetConnectionString("Default")
                    ?? throw new InvalidOperationException("Connection string 'Default' not found")))
            .AddScoped<IEventRepository, EventRepository>()
            .AddScoped<IBookingRepository, BookingRepository>();
    }

    public static DbContextOptionsBuilder ConfigureDatabase(this DbContextOptionsBuilder options, string connectionString)
    {
        return options
            .UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention();
    }
}
