using Microsoft.Azure.Cosmos;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Providers;

/// <summary>
/// Defines an interface for retrieving Cosmos DB containers.
/// </summary>
public interface ICosmosDbContainerProvider
{
    /// <summary>
    /// Retrieves a Cosmos DB container using the specified container key.
    /// </summary>
    /// <param name="containerKey">The key identifying the container to retrieve.</param>
    /// <returns>The requested Cosmos DB container.</returns>
    Task<Container> GetContainerAsync(string containerKey);
}