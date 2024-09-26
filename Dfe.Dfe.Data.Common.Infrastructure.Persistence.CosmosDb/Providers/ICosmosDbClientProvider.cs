using Microsoft.Azure.Cosmos;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Providers;

/// <summary>
/// 
/// </summary>
public interface ICosmosDbClientProvider
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="clientInvoker"></param>
    /// <returns></returns>
    Task<TItem> InvokeCosmosClientAsync<TItem>(Func<CosmosClient, Task<TItem>> clientInvoker);
}
