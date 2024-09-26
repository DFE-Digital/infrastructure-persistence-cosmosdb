namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Repositories;

/// <summary>
/// 
/// </summary>
public interface IReadOnlyRepository
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="id"></param>
    /// <param name="containerKey"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TItem> GetItemByIdAsync<TItem>(
        string id, string containerKey, CancellationToken cancellationToken = default) where TItem : class;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="id"></param>
    /// <param name="containerKey"></param>
    /// <param name="partitionKey"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TItem> GetItemByIdAsync<TItem>(
        string id, string containerKey, string partitionKey, CancellationToken cancellationToken = default) where TItem : class;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="query"></param>
    /// <param name="containerKey"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IEnumerable<TItem>> GetAllItemsByQueryAsync<TItem>(
        string query, string containerKey, CancellationToken cancellationToken = default) where TItem : class;
}
