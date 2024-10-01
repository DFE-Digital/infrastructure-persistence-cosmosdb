using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Providers;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System.Linq.Expressions;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Handlers.Query;

/// <summary>
/// 
/// </summary>
public sealed class CosmosDbQueryHandler : ICosmosDbQueryHandler
{
    private readonly ICosmosDbContainerProvider _cosmosDbContainerProvider;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cosmosDbContainerProvider"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public CosmosDbQueryHandler(ICosmosDbContainerProvider cosmosDbContainerProvider)
    {
        _cosmosDbContainerProvider = cosmosDbContainerProvider ??
            throw new ArgumentNullException(nameof(cosmosDbContainerProvider));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="id"></param>
    /// <param name="containerKey"></param>
    /// <param name="partitionKeyValue"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<TItem> ReadItemByIdAsync<TItem>(
        string id,
        string containerKey,
        string partitionKeyValue,
        CancellationToken cancellationToken = default) where TItem : class
    {
        Container container =
            await _cosmosDbContainerProvider
                .GetContainerAsync(containerKey).ConfigureAwait(false);

        ItemResponse<TItem> response =
            await container.ReadItemAsync<TItem>(
                id, new PartitionKey(partitionKeyValue),
                cancellationToken: cancellationToken).ConfigureAwait(false);

        return response.Resource;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="containerKey"></param>
    /// <param name="selector"></param>
    /// <param name="predicate"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
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
            container.GetItemLinqQueryable<TItem>()
                .Where(predicate)
                .Select(selector)
                .ToFeedIterator(), cancellationToken);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="containerKey"></param>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
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
    /// 
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="feedIterator"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IEnumerable<TItem>> ReadItemsAsync<TItem>(
        FeedIterator<TItem> feedIterator,
        CancellationToken cancellationToken = default) where TItem : class
    {
        var items = new List<TItem>();

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
