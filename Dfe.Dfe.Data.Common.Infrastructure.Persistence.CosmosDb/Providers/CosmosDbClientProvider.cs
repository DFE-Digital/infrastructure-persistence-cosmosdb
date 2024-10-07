using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Options;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Providers;

/// <summary>
/// 
/// </summary>
public sealed class CosmosDbClientProvider : ICosmosDbClientProvider, IDisposable
{
    private readonly Lazy<CosmosClient> _lazyCosmosClient;
    private readonly RepositoryOptions _repositoryOptions;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="repositoryOptions"></param>
    public CosmosDbClientProvider(IOptions<RepositoryOptions> repositoryOptions)
    {
        ArgumentNullException.ThrowIfNull(repositoryOptions);

        _repositoryOptions = repositoryOptions.Value;
        _lazyCosmosClient = new Lazy<CosmosClient>(CreateCosmosClientInstance);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="clientInvoker"></param>
    /// <returns></returns>
    public Task<TItem> InvokeCosmosClientAsync<TItem>(
        Func<CosmosClient, Task<TItem>> clientInvoker) =>
            clientInvoker.Invoke(_lazyCosmosClient.Value);

    /// <summary>
    /// 
    /// </summary>
    public void Dispose()
    {
        if (_lazyCosmosClient.IsValueCreated){
            _lazyCosmosClient.Value?.Dispose();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private CosmosClient CreateCosmosClientInstance() =>
        new(
            _repositoryOptions.EndpointUri,
            _repositoryOptions.PrimaryKey,
            new CosmosClientOptions() {
                ConnectionMode = (ConnectionMode)_repositoryOptions.ConnectionMode
            });
}
