using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Options;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Options.Extensions;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Providers;

/// <summary>
/// Provides a mechanism for retrieving Cosmos DB containers
/// by implementing the <see cref="ICosmosDbContainerProvider"/> interface.
/// </summary>
public sealed class CosmosDbContainerProvider : ICosmosDbContainerProvider
{
    private readonly ILogger<CosmosDbContainerProvider> _logger;
    private readonly ICosmosDbClientProvider _cosmosClientProvider;
    private readonly RepositoryOptions _repositoryOptions;

    /// <summary>
    /// Initializes a new instance of <see cref="CosmosDbContainerProvider"/>.
    /// </summary>
    /// <param name="logger">Logger for capturing diagnostic information.</param>
    /// <param name="cosmosClientProvider">Provider for managing Cosmos DB client operations.</param>
    /// <param name="repositoryOptions">Configuration options for Cosmos DB containers.</param>
    /// <exception cref="ArgumentNullException">Thrown if any dependencies are null.</exception>
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
    /// Retrieves a Cosmos DB container using the specified container key.
    /// </summary>
    /// <param name="containerKey">The container key used to identify the target container.</param>
    /// <returns>The retrieved Cosmos DB container.</returns>
    public async Task<Container> GetContainerAsync(string containerKey)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(containerKey);

        try
        {
            // Retrieve container configuration options.
            ContainerOptions containerOptions =
                _repositoryOptions.GetContainerOptions(containerKey);

            // Ensure the database exists and retrieve it.
            Database database =
                await _cosmosClientProvider.InvokeCosmosClientAsync(client =>
                    client.CreateDatabaseIfNotExistsAsync(
                        _repositoryOptions.DatabaseId)).ConfigureAwait(false);

            // Ensure the container exists and return it.
            return (Container)await
                database.CreateContainerIfNotExistsAsync(
                    containerOptions.ContainerName, containerOptions.PartitionKey).ConfigureAwait(false);
        }
        catch (CosmosException cosmosEx)
        {
            _logger.LogError(
                cosmosEx, "A Cosmos DB error occurred while retrieving the specified container.");

            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex, "An error occurred while retrieving the specified container.");

            throw;
        }
    }
}