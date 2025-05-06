using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Handlers.Query;

/// <summary>
/// Represents a LINQ query for Cosmos DB by implementing
/// the <see cref="IQueryableToFeedIterator"/> interface.
/// </summary>
public sealed class QueryableToFeedIterator : IQueryableToFeedIterator
{
    /// <summary>
    /// Creates a FeedIterator from an IQueryable.
    /// </summary>
    /// <typeparam name="TItem">The type of items in the query.</typeparam>
    /// <param name="query">The LINQ query to convert to a FeedIterator.</param>
    /// <returns>A FeedIterator for the given query.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the query is null.</exception>
    public FeedIterator<TItem> GetFeedIterator<TItem>(IQueryable<TItem> query)
    {
        // Ensure the query parameter is not null.
        ArgumentNullException.ThrowIfNull(nameof(query));

        // Convert the IQueryable to a FeedIterator and return it.
        return query.ToFeedIterator();
    }
}
