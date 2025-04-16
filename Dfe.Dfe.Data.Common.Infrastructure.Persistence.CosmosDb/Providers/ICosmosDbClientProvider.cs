using Microsoft.Azure.Cosmos;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Providers;

/// <summary>
/// Defines an interface for interacting with an Azure Cosmos DB client.
/// </summary>
public interface ICosmosDbClientProvider
{
    /// <summary>
    /// Executes an operation using the Cosmos DB client.
    /// </summary>
    /// <typeparam name="TItem">Type of data returned by the operation.</typeparam>
    /// <param name="clientInvoker">A function that performs an operation using the Cosmos DB client.</param>
    /// <returns>The result of the invoked operation.</returns>
    Task<TItem> InvokeCosmosClientAsync<TItem>(Func<CosmosClient, Task<TItem>> clientInvoker);
}