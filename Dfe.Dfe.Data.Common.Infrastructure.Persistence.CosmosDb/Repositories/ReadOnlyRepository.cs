using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Handlers.Query;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Repositories;

/// <summary>
/// Implements read-only operations for interacting with Azure Cosmos DB.
/// </summary>
public sealed class ReadOnlyRepository : IReadOnlyRepository
{
    private readonly ICosmosDbQueryHandler _cosmosDbQueryHandler;

    /// <summary>
    /// Initializes a new instance of <see cref="ReadOnlyRepository"/>.
    /// </summary>
    /// <param name="cosmosDbQueryHandler">The query handler used to interact with Cosmos DB.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="cosmosDbQueryHandler"/> is null.</exception>
    public ReadOnlyRepository(ICosmosDbQueryHandler cosmosDbQueryHandler)
    {
        _cosmosDbQueryHandler = cosmosDbQueryHandler ??
            throw new ArgumentNullException(nameof(cosmosDbQueryHandler));
    }

    /// <summary>
    /// Retrieves an item by its unique ID from Cosmos DB.
    /// </summary>
    /// <typeparam name="TItem">The type of item to retrieve.</typeparam>
    /// <param name="id">The unique identifier of the item.</param>
    /// <param name="containerKey">The key identifying the container where the item is stored.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>The requested item.</returns>
    public Task<TItem> GetItemByIdAsync<TItem>(
       string id,
       string containerKey,
       CancellationToken cancellationToken = default) where TItem : class =>
           _cosmosDbQueryHandler.ReadItemByIdAsync<TItem>(
               id, containerKey, partitionKeyValue: id, cancellationToken);

    /// <summary>
    /// Retrieves an item by its unique ID and partition key from Cosmos DB.
    /// </summary>
    /// <typeparam name="TItem">The type of item to retrieve.</typeparam>
    /// <param name="id">The unique identifier of the item.</param>
    /// <param name="containerKey">The key identifying the container where the item is stored.</param>
    /// <param name="partitionKey">The partition key value for efficient lookup.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>The requested item.</returns>
    public Task<TItem> GetItemByIdAsync<TItem>(
        string id,
        string containerKey,
        string partitionKey,
        CancellationToken cancellationToken = default) where TItem : class =>
            _cosmosDbQueryHandler.ReadItemByIdAsync<TItem>(
                id, containerKey, partitionKey, cancellationToken);

    /// <summary>
    /// Executes a query to retrieve multiple items from Cosmos DB.
    /// </summary>
    /// <typeparam name="TItem">The type of items to retrieve.</typeparam>
    /// <param name="query">The SQL-like query string used to filter results.</param>
    /// <param name="containerKey">The key identifying the container where the items are stored.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A collection of items matching the query.</returns>
    public Task<IEnumerable<TItem>> GetAllItemsByQueryAsync<TItem>(
        string query,
        string containerKey,
        CancellationToken cancellationToken = default) where TItem : class =>
            _cosmosDbQueryHandler.ReadItemsAsync<TItem>(
                containerKey, query, cancellationToken);
}