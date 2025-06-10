using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration.Fixture.Model;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration.Fixture;

/// <summary>
/// This class sets up a test fixture for integration testing with Cosmos DB.
/// </summary>
public class CosmosDbTestFixture<THandlerType> where THandlerType : class
{
    /// <summary>
    /// Public property exposing the query handler used to interact with Cosmos DB.
    /// </summary>
    public THandlerType? Handler { get; private set; }

    /// <summary>
    /// A collection of container records used for testing purposes.
    /// </summary>
    public IReadOnlyCollection<ContainerRecord>? ContainerRecords { get; private set; }

    /// <summary>
    /// Private field to manage the life-cycle of the Cosmos DB test helper.
    /// </summary>
    private CosmosDbTestHelper? _cosmosDbTestHelper;

    /// <summary>
    /// Initializes the Cosmos DB test fixture, creating an isolated query handler for testing.
    /// </summary>
    public async Task InitializeAsync(string containerName)
    {
        // Create a new instance of the test helper.
        _cosmosDbTestHelper = new CosmosDbTestHelper();

        // Use the helper to create an isolated query handler for testing.
        var (handler, _, records) =
            await _cosmosDbTestHelper.CreateIsolatedDbContext<THandlerType>(containerName);

        // Assign the created query handler and created records to the associated properties.
        Handler = handler;
        ContainerRecords = records;
    }

    /// <summary>
    /// Disposes of the Cosmos DB test helper, cleaning up resources used during testing.
    /// </summary>
    public void Dispose() => _cosmosDbTestHelper?.Dispose();
}
