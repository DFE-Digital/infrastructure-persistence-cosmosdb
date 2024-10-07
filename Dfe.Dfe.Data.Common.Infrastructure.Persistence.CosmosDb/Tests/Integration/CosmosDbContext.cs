using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Options;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration.Model;
using Microsoft.Azure.Cosmos;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration;

/// <summary>
/// 
/// </summary>
public sealed class CosmosDbContext
{
    /// <summary>
    /// 
    /// </summary>
    public CosmosClient Client { get; private set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    public Database DatabaseInstance { get; private set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TContainerRecord"></typeparam>
    /// <param name="containerRecords"></param>
    /// <returns></returns>
    public async Task CreateAndPopulate<TContainerRecord>(
        RepositoryOptions repositoryOptions,
        IReadOnlyCollection<TContainerRecord> containerRecords)
        where TContainerRecord : class, IContainerRecord
    {
        Client =
            new CosmosClient(
                repositoryOptions.EndpointUri,
                repositoryOptions.PrimaryKey,
                new CosmosClientOptions()
                {
                    AllowBulkExecution = true,
                    ConnectionMode = ConnectionMode.Gateway
                });

        DatabaseInstance =
            await Client.CreateDatabaseIfNotExistsAsync(repositoryOptions.DatabaseId);

        try
        {
            string? containerKey =
                repositoryOptions?.Containers?[0].First().Key;

            await DatabaseInstance.DefineContainer(containerKey, "/pk")
                .WithIndexingPolicy()
                .WithIndexingMode(IndexingMode.Consistent)
                .WithIncludedPaths()
                    .Attach()
                .WithExcludedPaths()
                .Path("/*")
                    .Attach()
                .Attach()
                .CreateAsync(containerRecords.Count);

            Container container = DatabaseInstance.GetContainer(containerKey);
            List<Task> createRecordTasks = new(containerRecords.Count);

            containerRecords.ToList()
                .ForEach(containerRecord =>
                    createRecordTasks.Add(
                        container.CreateItemAsync(containerRecord, new PartitionKey(containerRecord.id))));

            await Task.WhenAll(createRecordTasks);
        }
        finally
        {
            CleanUpResources();
        }
    }

    private bool _clientDisposed;

    /// <summary>
    /// 
    /// </summary>
    public void CleanUpResources()
    {
        if (!_clientDisposed)
        {
            Task.Run(async () =>
                await DatabaseInstance.DeleteAsync()).Wait();

            Client?.Dispose();

            _clientDisposed = true;
        }
    }
}
