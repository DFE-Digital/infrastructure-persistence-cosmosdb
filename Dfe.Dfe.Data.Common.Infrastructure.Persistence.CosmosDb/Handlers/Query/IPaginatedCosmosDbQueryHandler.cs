using System.Linq.Expressions;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Handlers.Query;

/// <summary>
/// Defines operations for paginated queries in Azure Cosmos DB.
/// </summary>
public interface IPaginatedCosmosDbQueryHandler
{
    /// <summary>
    /// Retrieves a paginated set of items from Cosmos DB.
    /// </summary>
    /// <typeparam name="TItem">The type of items to retrieve.</typeparam>
    /// <param name="containerKey">The container key where the items are stored.</param>
    /// <param name="selector">Projection expression for selecting specific fields.</param>
    /// <param name="predicate">Filtering condition to limit results.</param>
    /// <param name="pageNumber">The page number for pagination.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A collection of items based on the specified pagination.</returns>
    Task<IEnumerable<TItem>> ReadPaginatedItemsAsync<TItem>(
        string containerKey,
        Expression<Func<TItem, TItem>> selector,
        Expression<Func<TItem, bool>> predicate,
        int pageNumber, int pageSize,
        CancellationToken cancellationToken = default) where TItem : class;

    /// <summary>
    /// Retrieves the total count of items matching a specified condition.
    /// </summary>
    /// <typeparam name="TItem">The type of items to count.</typeparam>
    /// <param name="containerKey">The container key where the items are stored.</param>
    /// <param name="predicate">Filtering condition for counting items.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>The number of matching items in the database.</returns>
    Task<int> GetItemCountAsync<TItem>(
        string containerKey,
        Expression<Func<TItem, bool>> predicate,
        CancellationToken cancellationToken = default) where TItem : class;
}