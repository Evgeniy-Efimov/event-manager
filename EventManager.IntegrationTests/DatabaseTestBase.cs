using EventManager.IntegrationTests.Fixtures;

namespace EventManager.IntegrationTests;

[Collection("Database")]
public abstract class DatabaseTestBase(PostgreSqlContainerFixture fixture) : IAsyncLifetime
{
    protected PostgreSqlContainerFixture Fixture { get; } = fixture;
    public Task InitializeAsync() => Fixture.ResetDatabaseAsync();
    public Task DisposeAsync() => Task.CompletedTask;
}
