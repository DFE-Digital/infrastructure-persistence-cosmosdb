using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Options;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration.Fixture.Model;
using Microsoft.Azure.Cosmos;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration.Fixture;

/// <summary>
/// Provides functionality to create and populate a Cosmos DB instance for integration testing.
/// </summary>
public sealed class CosmosDbContext
{
    /// <summary>
    /// The Cosmos DB client instance used for testing.
    /// </summary>
    public CosmosClient Client { get; private set; } = null!;

    /// <summary>
    /// The Cosmos DB database instance used for testing.
    /// </summary>
    public Database DatabaseInstance { get; private set; } = null!;

    /// <summary>
    /// Creates a Cosmos DB database and container, then populates it with the provided records.
    /// </summary>
    /// <typeparam name="TContainerRecord">The type of records to insert into the container.</typeparam>
    /// <param name="repositoryOptions">Configuration options for the Cosmos DB setup.</param>
    /// <param name="containerRecords">The records to insert into the container.</param>
    public async Task CreateAndPopulate<TContainerRecord>(
        RepositoryOptions repositoryOptions,
        IReadOnlyCollection<TContainerRecord> containerRecords)
        where TContainerRecord : class, IContainerRecord
    {
        // Initialize the Cosmos DB client with bulk execution and gateway mode
        Client = new CosmosClient(
            repositoryOptions.EndpointUri,
            repositoryOptions.PrimaryKey,
            new CosmosClientOptions()
            {
                AllowBulkExecution = true,
                ConnectionMode = ConnectionMode.Gateway
            });

        // Create the database if it doesn't already exist
        DatabaseInstance = await Client.CreateDatabaseIfNotExistsAsync(repositoryOptions.DatabaseId);

        // Get the container key from the first container configuration
        string? containerKey = repositoryOptions?.Containers?[0].First().Key;
        // Define and create the container with a consistent indexing policy
        await DatabaseInstance.DefineContainer(containerKey, "/pk")
            .WithIndexingPolicy()
            .WithIndexingMode(IndexingMode.Consistent)
            .WithIncludedPaths() // No specific included paths defined
                .Attach()
            .WithExcludedPaths() // Exclude all paths from indexing
                .Path("/*")
                .Attach()
            .Attach()
            .CreateAsync(containerRecords.Count); // Set throughput based on record count

        // Get a reference to the newly created container
        Container container = DatabaseInstance.GetContainer(containerKey);

        // Prepare tasks to insert each record into the container
        List<Task> createRecordTasks = new(containerRecords.Count);

        // Add a create item task for each record
        containerRecords.ToList()
            .ForEach(containerRecord =>
                createRecordTasks.Add(
                    container.CreateItemAsync(containerRecord, new PartitionKey(containerRecord.id))));

        // Wait for all insert operations to complete
        await Task.WhenAll(createRecordTasks);
    }

    // Flag to ensure cleanup is only performed once
    private bool _clientDisposed;

    /// <summary>
    /// Cleans up the Cosmos DB resources by deleting the database and disposing the client.
    /// </summary>
    public void CleanUpResources()
    {
        if (!_clientDisposed)
        {
            // Delete the database asynchronously and wait for it to complete
            Task.Run(async () =>
                await DatabaseInstance.DeleteAsync()).Wait();

            // Dispose the Cosmos DB client
            Client?.Dispose();

            _clientDisposed = true;
        }
    }
}
