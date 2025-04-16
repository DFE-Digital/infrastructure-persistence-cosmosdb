using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Providers;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System.Linq.Expressions;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Handlers.Query;

/// <summary>
/// Handles queries to Azure Cosmos DB by implementing
/// the <see cref="ICosmosDbQueryHandler"/> interface.
/// </summary>
public sealed class CosmosDbQueryHandler : ICosmosDbQueryHandler
{
    private readonly ICosmosDbContainerProvider _cosmosDbContainerProvider;

    /// <summary>
    /// Initializes a new instance of CosmosDbQueryHandler.
    /// </summary>
    /// <param name="cosmosDbContainerProvider">Provider to get Cosmos DB containers.</param>
    /// <exception cref="ArgumentNullException">Thrown if provider is null.</exception>
    public CosmosDbQueryHandler(ICosmosDbContainerProvider cosmosDbContainerProvider)
    {
        _cosmosDbContainerProvider = cosmosDbContainerProvider ??
            throw new ArgumentNullException(nameof(cosmosDbContainerProvider));
    }

    /// <summary>
    /// Reads a single item from Cosmos DB using its ID.
    /// </summary>
    /// <typeparam name="TItem">The type of item to read.</typeparam>
    /// <param name="id">ID of the item.</param>
    /// <param name="containerKey">The container key to locate the item.</param>
    /// <param name="partitionKeyValue">Partition key value.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>The requested item.</returns>
    public async Task<TItem> ReadItemByIdAsync<TItem>(
        string id,
        string containerKey,
        string partitionKeyValue,
        CancellationToken cancellationToken = default) where TItem : class
    {
        // Retrieve the container where the item is stored.
        Container container =
            await _cosmosDbContainerProvider
                .GetContainerAsync(containerKey).ConfigureAwait(false);

        // Read the item using its ID and partition key.
        ItemResponse<TItem> response =
            await container.ReadItemAsync<TItem>(
                id, new PartitionKey(partitionKeyValue),
                cancellationToken: cancellationToken).ConfigureAwait(false);

        return response.Resource;
    }

    /// <summary>
    /// Reads multiple items matching a predicate from Cosmos DB.
    /// </summary>
    /// <typeparam name="TItem">Type of items to retrieve.</typeparam>
    /// <param name="containerKey">Container key to query.</param>
    /// <param name="selector">Projection expression for selecting specific fields.</param>
    /// <param name="predicate">Filtering condition.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A collection of matching items.</returns>
    public async Task<IEnumerable<TItem>> ReadItemsAsync<TItem>(
        string containerKey,
        Expression<Func<TItem, TItem>> selector,
        Expression<Func<TItem, bool>> predicate,
        CancellationToken cancellationToken = default) where TItem : class
    {
        Container container =
            await _cosmosDbContainerProvider
                .GetContainerAsync(containerKey).ConfigureAwait(false);

        return await ReadItemsAsync(
            container.GetItemLinqQueryable<TItem>() // Using LINQ to query Cosmos DB.
                .Where(predicate)  // Filtering items.
                .Select(selector)  // Selecting specific fields.
                .ToFeedIterator(), cancellationToken);
    }

    /// <summary>
    /// Reads multiple items using a raw query string.
    /// </summary>
    /// <typeparam name="TItem">Type of items to retrieve.</typeparam>
    /// <param name="containerKey">Container key to query.</param>
    /// <param name="query">SQL query string.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A collection of retrieved items.</returns>
    public async Task<IEnumerable<TItem>> ReadItemsAsync<TItem>(
        string containerKey,
        string query,
        CancellationToken cancellationToken = default) where TItem : class
    {
        Container container =
            await _cosmosDbContainerProvider
                .GetContainerAsync(containerKey).ConfigureAwait(false);

        return await ReadItemsAsync(
            container.GetItemQueryIterator<TItem>(new QueryDefinition(query)), cancellationToken);
    }

    /// <summary>
    /// Reads items asynchronously using a feed iterator.
    /// </summary>
    /// <typeparam name="TItem">Type of items being retrieved.</typeparam>
    /// <param name="feedIterator">Iterator for consuming query results.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A list of retrieved items.</returns>
    public async Task<IEnumerable<TItem>> ReadItemsAsync<TItem>(
        FeedIterator<TItem> feedIterator,
        CancellationToken cancellationToken = default) where TItem : class
    {
        var items = new List<TItem>();

        using (feedIterator) // Ensure proper disposal of the iterator.
        {
            while (feedIterator.HasMoreResults) // Fetch results in batches.
            {
                FeedResponse<TItem> response =
                    await feedIterator
                        .ReadNextAsync(cancellationToken).ConfigureAwait(false);

                items.AddRange(response.Resource); // Aggregate results.
            }
        }

        return items;
    }
}