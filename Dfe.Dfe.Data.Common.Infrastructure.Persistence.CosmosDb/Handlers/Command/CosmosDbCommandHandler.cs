using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Providers;
using Microsoft.Azure.Cosmos;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Handlers.Command;

/// <summary>
/// Handles create, update, and delete operations for Cosmos DB items.
/// Uses a container provider to resolve containers by key.
/// </summary>
public sealed class CosmosDbCommandHandler : ICosmosDbCommandHandler
{
    private readonly ICosmosDbContainerProvider _cosmosDbContainerProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="CosmosDbCommandHandler"/> class
    /// allowing command type (data mutation) operations to be performed.
    /// </summary>
    /// <param name="cosmosDbContainerProvider">The provider used to retrieve Cosmos DB containers.</param>
    public CosmosDbCommandHandler(ICosmosDbContainerProvider cosmosDbContainerProvider)
    {
        _cosmosDbContainerProvider = cosmosDbContainerProvider ??
            throw new ArgumentNullException(nameof(cosmosDbContainerProvider));
    }

    /// <summary>
    /// Creates a new item in the specified container using a string partition key.
    /// </summary>
    /// <typeparam name="TItem">The type of the item to create.</typeparam>
    /// <param name="item">The item to create.</param>
    /// <param name="containerKey">The key used to resolve the Cosmos DB container.</param>
    /// <param name="partitionKeyValue">The value of the partition key.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The created item.</returns>
    public Task<TItem> CreateItemAsync<TItem>(
        TItem item,
        string containerKey,
        string partitionKeyValue,
        CancellationToken cancellationToken = default) where TItem : class
    {
        ArgumentNullException.ThrowIfNullOrEmpty(partitionKeyValue);

        return CreateItemAsync(item, containerKey, new PartitionKey(partitionKeyValue), cancellationToken);
    }

    /// <summary>
    /// Creates a new item in the specified container using a <see cref="PartitionKey"/>.
    /// </summary>
    /// <typeparam name="TItem">The type of the item to create.</typeparam>
    /// <param name="item">The item to create.</param>
    /// <param name="containerKey">The key used to resolve the Cosmos DB container.</param>
    /// <param name="partitionKey">The partition key for the item.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The created item.</returns>
    public async Task<TItem> CreateItemAsync<TItem>(
        TItem item,
        string containerKey,
        PartitionKey partitionKey,
        CancellationToken cancellationToken = default) where TItem : class
    {
        ArgumentNullException.ThrowIfNull(item);
        ArgumentNullException.ThrowIfNullOrEmpty(containerKey);
        ArgumentNullException.ThrowIfNull(partitionKey);

        var container = await _cosmosDbContainerProvider
            .GetContainerAsync(containerKey).ConfigureAwait(false);

        var response = await container.CreateItemAsync(
            item, partitionKey, cancellationToken: cancellationToken).ConfigureAwait(false);

        return response.Resource;
    }

    /// <summary>
    /// Updates or inserts an item using a string partition key.
    /// </summary>
    /// <typeparam name="TItem">The type of the item to update.</typeparam>
    /// <param name="item">The item to update.</param>
    /// <param name="containerKey">The key used to resolve the Cosmos DB container.</param>
    /// <param name="partitionKeyValue">The value of the partition key.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated item.</returns>
    public Task<TItem> UpdateItemAsync<TItem>(
        TItem item,
        string containerKey,
        string partitionKeyValue,
        CancellationToken cancellationToken = default) where TItem : class
    {
        ArgumentNullException.ThrowIfNullOrEmpty(partitionKeyValue);

        return UpdateItemAsync(item, containerKey, new PartitionKey(partitionKeyValue), cancellationToken);
    }

    /// <summary>
    /// Updates or inserts an item using a <see cref="PartitionKey"/>.
    /// </summary>
    /// <typeparam name="TItem">The type of the item to update.</typeparam>
    /// <param name="item">The item to update.</param>
    /// <param name="containerKey">The key used to resolve the Cosmos DB container.</param>
    /// <param name="partitionKey">The partition key for the item.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated item.</returns>
    public async Task<TItem> UpdateItemAsync<TItem>(
        TItem item,
        string containerKey,
        PartitionKey partitionKey,
        CancellationToken cancellationToken = default) where TItem : class
    {
        ArgumentNullException.ThrowIfNull(item);
        ArgumentNullException.ThrowIfNullOrEmpty(containerKey);
        ArgumentNullException.ThrowIfNull(partitionKey);

        var container = await _cosmosDbContainerProvider
            .GetContainerAsync(containerKey).ConfigureAwait(false);

        var response = await container.UpsertItemAsync(
            item, partitionKey, cancellationToken: cancellationToken).ConfigureAwait(false);

        return response.Resource;
    }

    /// <summary>
    /// Deletes an item using a string partition key.
    /// </summary>
    /// <typeparam name="TItem">The type of the item to delete.</typeparam>
    /// <param name="id">The ID of the item to delete.</param>
    /// <param name="containerKey">The key used to resolve the Cosmos DB container.</param>
    /// <param name="partitionKeyValue">The value of the partition key.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    public Task DeleteItemAsync<TItem>(
        string id,
        string containerKey,
        string partitionKeyValue = null!,
        CancellationToken cancellationToken = default) where TItem : class
    {
        ArgumentNullException.ThrowIfNullOrEmpty(partitionKeyValue);

        return DeleteItemAsync<TItem>(
            id, containerKey, new PartitionKey(partitionKeyValue ?? id), cancellationToken);
    }

    /// <summary>
    /// Deletes an item using a <see cref="PartitionKey"/>.
    /// </summary>
    /// <typeparam name="TItem">The type of the item to delete.</typeparam>
    /// <param name="id">The ID of the item to delete.</param>
    /// <param name="containerKey">The key used to resolve the Cosmos DB container.</param>
    /// <param name="partitionKey">The partition key for the item.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    public async Task DeleteItemAsync<TItem>(
        string id,
        string containerKey,
        PartitionKey partitionKey,
        CancellationToken cancellationToken = default) where TItem : class
    {
        ArgumentNullException.ThrowIfNullOrEmpty(id);
        ArgumentNullException.ThrowIfNullOrEmpty(containerKey);
        ArgumentNullException.ThrowIfNull(partitionKey);

        var container = await _cosmosDbContainerProvider
            .GetContainerAsync(containerKey).ConfigureAwait(false);

        if (partitionKey.Equals(default))
            partitionKey = new PartitionKey(id);

        _ = await container.DeleteItemAsync<TItem>(
            id, partitionKey, cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
