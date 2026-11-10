using EventManager.DataAccess.DbContexts;
using EventManager.IntegrationTests.Fixtures;

namespace EventManager.IntegrationTests;

[Collection("PostgreSqlTests")]
public abstract class PostgreSqlTestBase(PostgreSqlContainerFixture fixture) : IAsyncLifetime
{
    protected PostgreSqlContainerFixture Fixture { get; } = fixture;
    protected AppDbContext CreateDbContext() => Fixture.CreateDbContext();
    public Task InitializeAsync() => Fixture.ResetDatabaseAsync();
    public Task DisposeAsync() => Task.CompletedTask;
}
