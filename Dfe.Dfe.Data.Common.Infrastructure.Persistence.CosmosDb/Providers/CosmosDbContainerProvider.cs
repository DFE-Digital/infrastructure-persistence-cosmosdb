using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Options;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Options.Extensions;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Providers;

/// <summary>
/// 
/// </summary>
public sealed class CosmosDbContainerProvider : ICosmosDbContainerProvider
{
    private readonly ILogger<CosmosDbContainerProvider> _logger;
    private readonly ICosmosDbClientProvider _cosmosClientProvider;
    private readonly RepositoryOptions _repositoryOptions;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="cosmosClientProvider"></param>
    /// <param name="repositoryOptions"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public CosmosDbContainerProvider(
        ILogger<CosmosDbContainerProvider> logger,
        ICosmosDbClientProvider cosmosClientProvider,
        IOptions<RepositoryOptions> repositoryOptions)
    {
        _logger = logger ??
            throw new ArgumentNullException(nameof(logger));

        _cosmosClientProvider = cosmosClientProvider ??
            throw new ArgumentNullException(nameof(cosmosClientProvider));

        ArgumentNullException.ThrowIfNull(repositoryOptions);

        _repositoryOptions = repositoryOptions.Value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="containerKey"></param>
    /// <returns></returns>
    public async Task<Container> GetContainerAsync(string containerKey)
    {
        try
        {
            ContainerOptions containerOptions =
                _repositoryOptions.GetContainerOptions(containerKey);

            Database database =
                await _cosmosClientProvider.InvokeCosmosClientAsync(
                    client =>
                        client.CreateDatabaseIfNotExistsAsync(
                            _repositoryOptions.DatabaseId)).ConfigureAwait(false);

            return (Container)await
                database.CreateContainerIfNotExistsAsync(
                    containerOptions.ContainerName, containerOptions.PartitionKey).ConfigureAwait(false);
        }
        catch (CosmosException cosmosEx)
        {
            _logger.LogError(
                cosmosEx, "A Cosmos db error has occurred retrieving the container specified.");

            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex, "An error has occurred retrieving the container specified.");

            throw;
        }
    }
}
