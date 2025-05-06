using Microsoft.Azure.Cosmos;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Handlers.Query;

/// <summary>
/// Interface for converting LINQ queries to FeedIterator instances.
/// </summary>
public interface IQueryableToFeedIterator
{
    /// <summary>
    /// Converts an IQueryable to a FeedIterator.
    /// </summary>
    /// <typeparam name="TItem">The type of items in the query.</typeparam>
    /// <param name="query">The IQueryable to convert.</param>
    /// <returns>A FeedIterator for the query.</returns>
    FeedIterator<TItem> GetFeedIterator<TItem>(IQueryable<TItem> query);
}