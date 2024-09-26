using Microsoft.Azure.Cosmos;
using System.Linq.Expressions;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Handlers.Query;

/// <summary>
/// 
/// </summary>
public interface ICosmosDbQueryHandler
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="id"></param>
    /// <param name="containerKey"></param>
    /// <param name="partitionKeyValue"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TItem> ReadItemByIdAsync<TItem>(
       string id,
       string containerKey,
       string partitionKeyValue,
       CancellationToken cancellationToken = default) where TItem : class;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="containerKey"></param>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IEnumerable<TItem>> ReadIterableItemsAsync<TItem>(
        string containerKey,
        string query,
        CancellationToken cancellationToken = default) where TItem : class;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="containerKey"></param>
    /// <param name="selector"></param>
    /// <param name="predicate"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IEnumerable<TItem>> ReadIterableItemsAsync<TItem>(
        string containerKey,
        Expression<Func<TItem, TItem>> selector,
        Expression<Func<TItem, bool>> predicate,
        CancellationToken cancellationToken = default) where TItem : class;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="feedIterator"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IEnumerable<TItem>> ReadIterableItemsAsync<TItem>(
        FeedIterator<TItem> feedIterator,
        CancellationToken cancellationToken = default) where TItem : class;
}