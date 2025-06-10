using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Options;
using Microsoft.Azure.Cosmos;
using System.Net;

namespace DfE.GIAP.Core.IntegrationTests.Fixture;

/// <summary>
/// Represents a disposable Cosmos DB test database used for integration testing.
/// </summary>
public sealed class CosmosDbTestDatabase : IAsyncDisposable
{
    private readonly CosmosClient _cosmosClient;
    private readonly string _databaseId;
    private readonly string _containerName;

    /// <summary>
    /// Initializes a new instance of the <see cref="CosmosDbTestDatabase"/> class.
    /// </summary>
    /// <param name="options">Repository options containing Cosmos DB credentials.</param>
    /// <param name="databaseId">The ID of the test database.</param>
    /// <param name="containerName">The name of the container to use.</param>
    public CosmosDbTestDatabase(RepositoryOptions options, string databaseId, string containerName)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(databaseId);
        ArgumentNullException.ThrowIfNullOrWhiteSpace(containerName);

        _cosmosClient = new(
            accountEndpoint: options.EndpointUri,
            authKeyOrResourceToken: options.PrimaryKey);

        _databaseId = databaseId;
        _containerName = containerName;
    }

    /// <summary>
    /// Asynchronously disposes of the Cosmos DB client and deletes the test database.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        using (_cosmosClient)
        {
            await DeleteDatabase();
        }
    }

    /// <summary>
    /// Starts the test database by creating it and its containers.
    /// </summary>
    /// <param name="partitionKeyPath">The partition key path for the container.</param>
    public async Task StartAsync(string partitionKeyPath)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(partitionKeyPath);
        DatabaseResponse db = await CreateDatabase(_cosmosClient);
        await CreateAllContainers(db, partitionKeyPath);
    }

    /// <summary>
    /// Clears all items from the test database.
    /// </summary>
    /// <param name="partitionKeyPath">The partition key path for the container.</param>
    /// /// <param name="partitionKeyPath">The partition key value for the container.</param>
    public async Task ClearDatabaseAsync(string partitionKeyPath)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(partitionKeyPath);
        DatabaseResponse response = await CreateDatabase(_cosmosClient);
        List<ContainerResponse> containers = await CreateAllContainers(response, partitionKeyPath);

        foreach (ContainerResponse container in containers)
        {
            QueryDefinition queryDefinition = new("SELECT * FROM c");
            FeedIterator<dynamic> queryIterator = container.Container.GetItemQueryIterator<dynamic>(queryDefinition);

            List<Task> deleteTasks = [];

            while (queryIterator.HasMoreResults)
            {
                FeedResponse<dynamic> queriedItem = await queryIterator.ReadNextAsync();
                foreach (dynamic item in queriedItem)
                {
                    dynamic id = item.id.ToString();
                    PartitionKey partitionKey = new(item.id.ToString());

                    // Check if item exists before attempting deletion
                    dynamic itemExists = await container.Container.ReadItemAsync<dynamic>(id, partitionKey);
                    if (itemExists.StatusCode == HttpStatusCode.OK)
                    {
                        deleteTasks.Add(container.Container.DeleteItemAsync<dynamic>(id, partitionKey));
                    }
                }
            }

            await Task.WhenAll(deleteTasks);
            await container.Container.ReadContainerAsync();
        }
    }

    /// <summary>
    /// Deletes the test database.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task DeleteDatabase() => _cosmosClient!.GetDatabase(_databaseId).DeleteAsync();

    /// <summary>
    /// Writes an object to the container using its DocumentType as the partition key.
    /// </summary>
    /// <typeparam name="TResponse">The type of the object to write.</typeparam>
    /// <param name="obj">The object to write.</param>
    /// <param name="partitionKeyPath">The partition key path for the container.</param>
    public async Task WriteAsync<TResponse>(TResponse obj, string partitionKeyPath) where TResponse : class
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(partitionKeyPath);
        DatabaseResponse db = await CreateDatabase(_cosmosClient);
        List<ContainerResponse> containers = await CreateAllContainers(db, partitionKeyPath);
        ContainerResponse targetContainer =
            containers.Single(containerResponse =>
                containerResponse.Container.Id == _containerName);

        ItemResponse<TResponse> response =
            await targetContainer.Container
                .UpsertItemAsync(obj, new PartitionKey((obj as dynamic).id));

        Assert.Contains(response.StatusCode, new[] { HttpStatusCode.Created, HttpStatusCode.OK });
    }

    /// <summary>
    /// Creates the database if it does not already exist.
    /// </summary>
    /// <param name="client">The Cosmos DB client.</param>
    /// <returns>A task that returns the database response.</returns>
    private Task<DatabaseResponse> CreateDatabase(CosmosClient client)
    {
        ArgumentNullException.ThrowIfNull(client);
        return client!.CreateDatabaseIfNotExistsAsync(_databaseId);
    }

    /// <summary>
    /// Creates the container(s) if they do not already exist.
    /// </summary>
    /// <param name="database">The Cosmos DB database.</param>
    /// <param name="partitionKeyPath">The partition key path for the container.</param>
    /// <returns>A task that returns a list of container responses.</returns>
    private async Task<List<ContainerResponse>> CreateAllContainers(Database database, string partitionKeyPath)
    {
        ArgumentNullException.ThrowIfNull(database);
        ArgumentNullException.ThrowIfNullOrWhiteSpace(partitionKeyPath);

        List<ContainerResponse> containerResponses = [];

        ContainerResponse response = await database.CreateContainerIfNotExistsAsync(new ContainerProperties()
        {
            Id = _containerName,
            PartitionKeyPath = partitionKeyPath
        });

        containerResponses.Add(response);
        return containerResponses;
    }
}