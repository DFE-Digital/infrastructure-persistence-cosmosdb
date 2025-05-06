using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Providers;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System.Linq.Expressions;
using System.Net;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Handlers.Query;

/// <summary>
/// Handles paginated queries in Azure Cosmos DB.
/// </summary>
public sealed class PaginatedCosmosDbQueryHandler : QueryResultReader, IPaginatedCosmosDbQueryHandler
{
    private readonly IQueryableToFeedIterator _cosmosLinqQuery;
    private readonly ICosmosDbContainerProvider _cosmosDbContainerProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="PaginatedCosmosDbQueryHandler"/> class.
    /// </summary>
    /// <param name="cosmosLinqQuery">The Cosmos LINQ query interface.</param>
    /// <param name="cosmosDbContainerProvider">The Cosmos DB container provider interface.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="cosmosLinqQuery"/> or <paramref name="cosmosDbContainerProvider"/> is null.
    /// </exception>
    public PaginatedCosmosDbQueryHandler(
        IQueryableToFeedIterator cosmosLinqQuery,
        ICosmosDbContainerProvider cosmosDbContainerProvider)
    {
        // Ensure the cosmosLinqQuery parameter is not null.
        _cosmosLinqQuery = cosmosLinqQuery ??
            throw new ArgumentNullException(nameof(cosmosLinqQuery));
        // Ensure the cosmosDbContainerProvider parameter is not null.
        _cosmosDbContainerProvider = cosmosDbContainerProvider ??
            throw new ArgumentNullException(nameof(cosmosDbContainerProvider));
    }

    /// <summary>
    /// Retrieves a paginated set of items from Cosmos DB.
    /// </summary>
    /// <typeparam name="TItem">The type of items to retrieve.</typeparam>
    /// <param name="containerKey">Container key for querying.</param>
    /// <param name="selector">Projection expression for selecting specific fields.</param>
    /// <param name="predicate">Filtering condition.</param>
    /// <param name="pageNumber">Current page number.</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A collection of paginated items.</returns>
    public async Task<IEnumerable<TItem>> ReadPaginatedItemsAsync<TItem>(
        string containerKey,
        Expression<Func<TItem, TItem>> selector,
        Expression<Func<TItem, bool>> predicate,
        int pageNumber, int pageSize,
        CancellationToken cancellationToken = default) where TItem : class
    {
        // Retrieve the container where the items are stored.
        Container container =
            await _cosmosDbContainerProvider
                .GetContainerAsync(containerKey).ConfigureAwait(false);

        // Establish the LINQ query for the container.
        IQueryable<TItem> query =
            container.GetItemLinqQueryable<TItem>()
                .Where(predicate)                   // Apply the filtering criteria.
                .Select(selector)                   // Select specific fields.
                .Skip((pageNumber - 1) * pageSize)  // Skip items for pagination.
                .Take(pageSize);                    // Take required number of items.

        // Apply filtering, projection, pagination, and execute the query.
        return await ReadResultItemsAsync(
            _cosmosLinqQuery.GetFeedIterator(query), cancellationToken);
    }

    /// <summary>
    /// Retrieves the total count of items matching a condition.
    /// </summary>
    /// <typeparam name="TItem">The type of items to count.</typeparam>
    /// <param name="containerKey">Container key for querying.</param>
    /// <param name="predicate">Filtering condition.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>The total count of matching items.</returns>
    /// <exception cref="NullReferenceException">Thrown if the count retrieval fails.</exception>
    public async Task<int> GetItemCountAsync<TItem>(
        string containerKey,
        Expression<Func<TItem, bool>> predicate,
        CancellationToken cancellationToken = default) where TItem : class
    {
        // Retrieve the container where the items are stored.
        Container container =
            await _cosmosDbContainerProvider
                .GetContainerAsync(containerKey).ConfigureAwait(false);

        // Execute the query to count matching items.
        Response<int> result =
            await container.GetItemLinqQueryable<TItem>()
                .Where(predicate)               // Apply filtering.
                .CountAsync(cancellationToken); // Count matching items.

        // Return item count if successful; otherwise, throw an exception.
        return (result.StatusCode == HttpStatusCode.OK) ?
            result.Resource :
            throw new NullReferenceException(
                $"Unable to determine item count, status code: {result.StatusCode}.");
    }
}