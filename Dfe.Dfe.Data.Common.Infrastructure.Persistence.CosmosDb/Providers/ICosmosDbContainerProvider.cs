using Microsoft.Azure.Cosmos;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Providers;

/// <summary>
/// 
/// </summary>
public interface ICosmosDbContainerProvider
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="containerKey"></param>
    /// <returns></returns>
    Task<Container> GetContainerAsync(string containerKey);
}
