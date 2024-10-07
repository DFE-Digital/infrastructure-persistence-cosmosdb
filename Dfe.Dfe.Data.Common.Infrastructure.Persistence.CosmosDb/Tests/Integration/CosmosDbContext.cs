using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Options;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration.Model;
using Microsoft.Azure.Cosmos;
using System.Diagnostics.CodeAnalysis;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration;

[SuppressMessage("Microsoft.Performance", "CD1600: The class must have a documentation header.")]
[SuppressMessage("Microsoft.Performance", "ClassDocumentationHeader: The class must have a documentation header.")]
public sealed class CosmosDbContext
{
    [SuppressMessage("Microsoft.Performance", "CD1606: The property must have a documentation header.")]
    [SuppressMessage("Microsoft.Performance", "PropertyDocumentationHeader: The property must have a documentation header.")]
    public CosmosClient Client { get; private set; } = null!;

    [SuppressMessage("Microsoft.Performance", "CD1606: The property must have a documentation header.")]
    [SuppressMessage("Microsoft.Performance", "PropertyDocumentationHeader: The property must have a documentation header.")]
    public Database DatabaseInstance { get; private set; } = null!;

    [SuppressMessage("Microsoft.Performance", "CD1605: The method must have a documentation header.")]
    [SuppressMessage("Microsoft.Performance", "MethodDocumentationHeader: The method must have a documentation header.")]
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
                        container.CreateItemAsync(containerRecord, new PartitionKey(containerRecord.Id))));

            await Task.WhenAll(createRecordTasks);
        }
        catch
        {
            CleanUpResources();
        }
    }

    private bool _clientDisposed;

    [SuppressMessage("Microsoft.Performance", "CD1605: The method must have a documentation header.")]
    [SuppressMessage("Microsoft.Performance", "MethodDocumentationHeader: The method must have a documentation header.")]
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
