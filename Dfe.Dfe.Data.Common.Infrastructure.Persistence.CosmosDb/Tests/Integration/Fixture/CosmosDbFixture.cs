using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Options;

namespace DfE.GIAP.Core.IntegrationTests.Fixture;

/// <summary>
/// Provides a test fixture for Cosmos DB integration tests.
/// Initializes and cleans up a test database and container using the Cosmos DB emulator.
/// </summary>
public sealed class CosmosDbFixture : IAsyncLifetime
{
    /// <summary>
    /// The ID of the test database used in the Cosmos DB emulator.
    /// </summary>
    private const string databaseId = "integration-test";

    /// <summary>
    /// The name of the container used within the test database.
    /// </summary>
    private const string containerName = "integration-test-container";

    /// <summary>
    /// The partition key path used for the container.
    /// </summary>
    private const string partitionKeyPath = "/pk";

    /// <summary>
    /// Gets the test Cosmos DB instance.
    /// </summary>
    public CosmosDbTestDatabase? Database { get; private set; }

    /// <summary>
    /// Initializes the Cosmos DB test database and clears any existing data.
    /// </summary>
    /// <returns>A task representing the asynchronous initialization operation.</returns>
    public async Task InitializeAsync()
    {
        RepositoryOptions options = RepositoryOptionsFactory.LocalCosmosDbEmulator();
        Database = new(options, databaseId, containerName);
        await Database.StartAsync(partitionKeyPath);
        await Database.ClearDatabaseAsync(partitionKeyPath);
    }

    /// <summary>
    /// Disposes of the Cosmos DB test database after tests complete.
    /// </summary>
    /// <returns>A task representing the asynchronous disposal operation.</returns>
    public async Task DisposeAsync()
    {
        if (Database != null)
        {
            await Database.DisposeAsync();
        }
    }
}
