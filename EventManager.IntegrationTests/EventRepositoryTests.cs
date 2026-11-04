using EventManager.IntegrationTests.Fixtures;

namespace EventManager.IntegrationTests;

public class EventRepositoryTests(PostgreSqlContainerFixture fixture) : DatabaseTestBase(fixture)
{
}
