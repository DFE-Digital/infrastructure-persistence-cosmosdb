namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Repositories;

/// <summary>
/// Defines an interface for read-only operations on an Azure Cosmos DB repository.
/// </summary>
public interface IReadOnlyRepository
{
    /// <summary>
    /// Retrieves an item by its unique ID from Cosmos DB.
    /// </summary>
    /// <typeparam name="TItem">The type of item to retrieve.</typeparam>
    /// <param name="id">The unique identifier of the item.</param>
    /// <param name="containerKey">The key identifying the container where the item is stored.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>The requested item.</returns>
    Task<TItem> GetItemByIdAsync<TItem>(
        string id, string containerKey, CancellationToken cancellationToken = default) where TItem : class;

    /// <summary>
    /// Retrieves an item by its unique ID and partition key from Cosmos DB.
    /// </summary>
    /// <typeparam name="TItem">The type of item to retrieve.</typeparam>
    /// <param name="id">The unique identifier of the item.</param>
    /// <param name="containerKey">The key identifying the container where the item is stored.</param>
    /// <param name="partitionKey">The partition key value for efficient lookup.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>The requested item.</returns>
    Task<TItem> GetItemByIdAsync<TItem>(
        string id, string containerKey, string partitionKey, CancellationToken cancellationToken = default) where TItem : class;

    /// <summary>
    /// Executes a query to retrieve multiple items from Cosmos DB.
    /// </summary>
    /// <typeparam name="TItem">The type of items to retrieve.</typeparam>
    /// <param name="query">The SQL-like query string used to filter results.</param>
    /// <param name="containerKey">The key identifying the container where the items are stored.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A collection of items matching the query.</returns>
    Task<IEnumerable<TItem>> GetAllItemsByQueryAsync<TItem>(
        string query, string containerKey, CancellationToken cancellationToken = default) where TItem : class;
}