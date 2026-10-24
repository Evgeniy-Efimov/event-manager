using EventManager.Domain.DbContexts;
using EventManager.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EventManager.UnitTests.DbContexts;

public class TestAppDbContext(
    DbContextOptions<AppDbContext> options,
    string dbName,
    Event[] eventsTestData,
    Booking[] bookingsTestData) : AppDbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase(dbName);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Event>().HasData(eventsTestData);
        modelBuilder.Entity<Booking>().HasData(bookingsTestData);
    }
}
