using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Providers;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System.Linq.Expressions;
using System.Net;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Handlers.Query;

/// <summary>
/// 
/// </summary>
public sealed class PaginatedCosmosDbQueryHandler : IPaginatedCosmosDbQueryHandler
{
    private readonly ICosmosDbContainerProvider _cosmosDbContainerProvider;
    private readonly ICosmosDbQueryHandler _cosmosDbQueryHandler;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cosmosDbContainerProvider"></param>
    /// <param name="cosmosDbQueryHandler"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public PaginatedCosmosDbQueryHandler(
        ICosmosDbContainerProvider cosmosDbContainerProvider,
        ICosmosDbQueryHandler cosmosDbQueryHandler)
    {
        _cosmosDbContainerProvider = cosmosDbContainerProvider ??
            throw new ArgumentNullException(nameof(cosmosDbContainerProvider));
        _cosmosDbQueryHandler = cosmosDbQueryHandler ??
            throw new ArgumentNullException(nameof(cosmosDbQueryHandler));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="containerKey"></param>
    /// <param name="selector"></param>
    /// <param name="predicate"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IEnumerable<TItem>> ReadPaginatedItemsAsync<TItem>(
        string containerKey,
        Expression<Func<TItem, TItem>> selector,
        Expression<Func<TItem, bool>> predicate,
        int pageNumber, int pageSize,
        CancellationToken cancellationToken = default) where TItem : class
    {
        Container container =
            await _cosmosDbContainerProvider
                .GetContainerAsync(containerKey).ConfigureAwait(false);

        return await _cosmosDbQueryHandler.ReadItemsAsync(
            container.GetItemLinqQueryable<TItem>()
                .Where(predicate)
                .Select(selector)
                .Skip((pageNumber - 1) * pageSize).Take(pageSize)
            .ToFeedIterator(), cancellationToken);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="containerKey"></param>
    /// <param name="predicate"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    public async Task<int> GetItemCountAsync<TItem>(
        string containerKey,
        Expression<Func<TItem, bool>> predicate,
        CancellationToken cancellationToken = default) where TItem : class
    {
        Container container =
            await _cosmosDbContainerProvider
                .GetContainerAsync(containerKey).ConfigureAwait(false);

        Response<int> result =
            await container.GetItemLinqQueryable<TItem>()
                .Where(predicate)
                .CountAsync(cancellationToken);

        return (result.StatusCode == HttpStatusCode.OK) ?
            result.Resource :
            throw new NullReferenceException(
                $"Unable to determine item count, status code: {result.StatusCode}.");
    }
}
