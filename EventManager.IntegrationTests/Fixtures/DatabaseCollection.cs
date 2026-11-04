namespace EventManager.IntegrationTests.Fixtures;

[CollectionDefinition("Database")]
public class DatabaseCollection : ICollectionFixture<PostgreSqlContainerFixture>
{
}
