using Microsoft.Azure.Cosmos;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Handlers.Query;

/// <summary>
/// Abstract base class for reading items asynchronously from a FeedIterator.
/// Provides a default implementation that can be overridden by derived classes.
/// </summary>
public abstract class QueryResultReader
{
    /// <summary>
    /// Reads items asynchronously from a FeedIterator.
    /// This method is virtual and can be overridden by derived classes.
    /// </summary>
    /// <typeparam name="TItem">The type of items to read.</typeparam>
    /// <param name="feedIterator">The FeedIterator to read items from.</param>
    /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous read operation. The task result contains an IEnumerable of TItem.</returns>
    public virtual async Task<IEnumerable<TItem>> ReadResultItemsAsync<TItem>(
        FeedIterator<TItem> feedIterator,
        CancellationToken cancellationToken = default) where TItem : class
    {
        ArgumentNullException.ThrowIfNull(feedIterator);

        List<TItem> items = [];

        using (feedIterator)
        {
            while (feedIterator.HasMoreResults)
            {
                FeedResponse<TItem> response =
                    await feedIterator
                        .ReadNextAsync(cancellationToken).ConfigureAwait(false);

                items.AddRange(response.Resource);
            }
        }

        return items;
    }
}