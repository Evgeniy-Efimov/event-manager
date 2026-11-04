using EventManager.IntegrationTests.Fixtures;

namespace EventManager.IntegrationTests;

public class BookingRepositoryTests(PostgreSqlContainerFixture fixture) : DatabaseTestBase(fixture)
{
}
