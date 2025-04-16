using Microsoft.Azure.Cosmos;
using System.Linq.Expressions;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Handlers.Query;

/// <summary>
/// Defines query operations for interacting with Azure Cosmos DB.
/// </summary>
public interface ICosmosDbQueryHandler
{
    /// <summary>
    /// Reads a single item from Cosmos DB using its ID.
    /// </summary>
    /// <typeparam name="TItem">The type of item to read.</typeparam>
    /// <param name="id">The unique ID of the item.</param>
    /// <param name="containerKey">The container key where the item is stored.</param>
    /// <param name="partitionKeyValue">The partition key value for efficient retrieval.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>The requested item.</returns>
    Task<TItem> ReadItemByIdAsync<TItem>(
       string id,
       string containerKey,
       string partitionKeyValue,
       CancellationToken cancellationToken = default) where TItem : class;

    /// <summary>
    /// Reads multiple items using a raw query string.
    /// </summary>
    /// <typeparam name="TItem">The type of items to retrieve.</typeparam>
    /// <param name="containerKey">The container key to query.</param>
    /// <param name="query">SQL query string used to fetch data.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A collection of retrieved items.</returns>
    Task<IEnumerable<TItem>> ReadItemsAsync<TItem>(
        string containerKey,
        string query,
        CancellationToken cancellationToken = default) where TItem : class;

    /// <summary>
    /// Reads multiple items matching a predicate from Cosmos DB.
    /// </summary>
    /// <typeparam name="TItem">The type of items to retrieve.</typeparam>
    /// <param name="containerKey">The container key to query.</param>
    /// <param name="selector">Projection expression for selecting specific fields.</param>
    /// <param name="predicate">Filtering condition to limit results.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A collection of matching items.</returns>
    Task<IEnumerable<TItem>> ReadItemsAsync<TItem>(
        string containerKey,
        Expression<Func<TItem, TItem>> selector,
        Expression<Func<TItem, bool>> predicate,
        CancellationToken cancellationToken = default) where TItem : class;

    /// <summary>
    /// Reads items asynchronously using a feed iterator.
    /// </summary>
    /// <typeparam name="TItem">The type of items being retrieved.</typeparam>
    /// <param name="feedIterator">Iterator used to fetch items efficiently.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A list of retrieved items.</returns>
    Task<IEnumerable<TItem>> ReadItemsAsync<TItem>(
        FeedIterator<TItem> feedIterator,
        CancellationToken cancellationToken = default) where TItem : class;
}