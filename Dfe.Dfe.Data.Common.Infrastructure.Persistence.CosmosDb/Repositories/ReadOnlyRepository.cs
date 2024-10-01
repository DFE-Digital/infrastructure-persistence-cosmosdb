using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Handlers.Query;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Repositories;

/// <summary>
/// 
/// </summary>
public sealed class ReadOnlyRepository : IReadOnlyRepository
{
    private readonly ICosmosDbQueryHandler _cosmosDbQueryHandler;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cosmosDbQueryHandler"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public ReadOnlyRepository(ICosmosDbQueryHandler cosmosDbQueryHandler)
    {
        _cosmosDbQueryHandler = cosmosDbQueryHandler ??
            throw new ArgumentNullException(nameof(cosmosDbQueryHandler));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="id"></param>
    /// <param name="containerKey"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<TItem> GetItemByIdAsync<TItem>(
       string id,
       string containerKey,
       CancellationToken cancellationToken = default) where TItem : class =>
           _cosmosDbQueryHandler.ReadItemByIdAsync<TItem>(
               id, containerKey, partitionKeyValue: id,  cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="id"></param>
    /// <param name="containerKey"></param>
    /// <param name="partitionKey"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<TItem> GetItemByIdAsync<TItem>(
        string id,
        string containerKey,
        string partitionKey,
        CancellationToken cancellationToken = default) where TItem : class =>
            _cosmosDbQueryHandler.ReadItemByIdAsync<TItem>(
                id, containerKey, partitionKey, cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="query"></param>
    /// <param name="containerKey"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<IEnumerable<TItem>> GetAllItemsByQueryAsync<TItem>(
        string query,
        string containerKey,
        CancellationToken cancellationToken = default) where TItem : class =>
            _cosmosDbQueryHandler.ReadItemsAsync<TItem>(
                containerKey, query, cancellationToken);
}
