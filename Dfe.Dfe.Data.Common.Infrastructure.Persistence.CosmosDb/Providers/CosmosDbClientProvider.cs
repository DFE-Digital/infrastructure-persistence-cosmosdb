using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Options;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Providers;

/// <summary>
/// Provides a Cosmos DB client instance using a lazy-loading approach.
/// Implements <see cref="ICosmosDbClientProvider"/> and <see cref="IDisposable"/>.
/// </summary>
public sealed class CosmosDbClientProvider : ICosmosDbClientProvider, IDisposable
{
    private readonly Lazy<CosmosClient> _lazyCosmosClient;
    private readonly RepositoryOptions _repositoryOptions;

    /// <summary>
    /// Initializes a new instance of <see cref="CosmosDbClientProvider"/>.
    /// </summary>
    /// <param name="repositoryOptions">Configuration options for connecting to Cosmos DB.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="repositoryOptions"/> is null.</exception>
    public CosmosDbClientProvider(IOptions<RepositoryOptions> repositoryOptions)
    {
        ArgumentNullException.ThrowIfNull(repositoryOptions);

        _repositoryOptions = repositoryOptions.Value;
        _lazyCosmosClient = new Lazy<CosmosClient>(CreateCosmosClientInstance);
    }

    /// <summary>
    /// Invokes an operation using the Cosmos DB client.
    /// </summary>
    /// <typeparam name="TItem">Type of data returned by the operation.</typeparam>
    /// <param name="clientInvoker">A function that performs an operation using the Cosmos client.</param>
    /// <returns>The result of the invoked operation.</returns>
    public Task<TItem> InvokeCosmosClientAsync<TItem>(
        Func<CosmosClient, Task<TItem>> clientInvoker) =>
            clientInvoker.Invoke(_lazyCosmosClient.Value);

    /// <summary>
    /// Disposes the Cosmos DB client if it has been created.
    /// </summary>
    public void Dispose()
    {
        if (_lazyCosmosClient.IsValueCreated)
        {
            _lazyCosmosClient.Value?.Dispose();
        }
    }

    /// <summary>
    /// Creates a new instance of <see cref="CosmosClient"/> using the configured options.
    /// </summary>
    /// <returns>A Cosmos DB client instance.</returns>
    private CosmosClient CreateCosmosClientInstance() =>
        new(
            _repositoryOptions.EndpointUri,
            _repositoryOptions.PrimaryKey,
            new CosmosClientOptions()
            {
                ConnectionMode = ConnectionMode.Gateway
            });
}