using EventManager.IntegrationTests.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace EventManager.IntegrationTests;

public class MigrationsTests(PostgreSqlContainerFixture fixture) : PostgreSqlTestBase(fixture)
{
    [Fact]
    public async Task GetMigrations_ReturnsAppliedInitialWithoutPending()
    {
        // Arrange
        using var context = CreateDbContext();

        // Act
        var appliedMigrations = await context.Database.GetAppliedMigrationsAsync();
        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();

        // Assert
        var initialCreateMigration = appliedMigrations.FirstOrDefault(m => m.EndsWith("_InitialCreate"));
        Assert.NotNull(initialCreateMigration);
        Assert.Empty(pendingMigrations);
    }
}
