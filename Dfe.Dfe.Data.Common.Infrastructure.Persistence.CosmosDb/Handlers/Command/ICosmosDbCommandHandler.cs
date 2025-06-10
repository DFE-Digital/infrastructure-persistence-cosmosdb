using Microsoft.Azure.Cosmos;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Handlers.Command;

/// <summary>
/// Provides an abstraction for mutating cosmos db container
/// items either using create, update, and delete operations.
/// </summary>
public interface ICosmosDbCommandHandler
{
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
        CancellationToken cancellationToken = default) where TItem : class;

    /// <summary>
    /// Creates a new item in the specified container using a <see cref="PartitionKey"/>.
    /// </summary>
    /// <typeparam name="TItem">The type of the item to create.</typeparam>
    /// <param name="item">The item to create.</param>
    /// <param name="containerKey">The key used to resolve the Cosmos DB container.</param>
    /// <param name="partitionKey">The partition key for the item.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The created item.</returns>
    public Task<TItem> CreateItemAsync<TItem>(
        TItem item,
        string containerKey,
        PartitionKey partitionKey,
        CancellationToken cancellationToken = default) where TItem : class;

    /// <summary>
    /// Updates or inserts an item using a string partition key.
    /// </summary>
    /// <typeparam name="TItem">The type of the item to update.</typeparam>
    /// <param name="item">The item to update.</param>
    /// <param name="containerKey">The key used to resolve the Cosmos DB container.</param>
    /// <param name="partitionKeyValue">The value of the partition key.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated item.</returns>
    public Task<TItem> UpsertItemAsync<TItem>(
        TItem item,
        string containerKey,
        string partitionKeyValue,
        CancellationToken cancellationToken = default) where TItem : class;

    /// <summary>
    /// Updates or inserts an item using a <see cref="PartitionKey"/>.
    /// </summary>
    /// <typeparam name="TItem">The type of the item to update.</typeparam>
    /// <param name="item">The item to update.</param>
    /// <param name="containerKey">The key used to resolve the Cosmos DB container.</param>
    /// <param name="partitionKey">The partition key for the item.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated item.</returns>
    public Task<TItem> UpsertItemAsync<TItem>(
        TItem item,
        string containerKey,
        PartitionKey partitionKey,
        CancellationToken cancellationToken = default) where TItem : class;

    /// <summary>
    /// Replaces an item using a string partition key.
    /// </summary>
    /// <typeparam name="TItem">The type of the item to update.</typeparam>
    /// <param name="item">The item to update.</param>
    /// <param name="itemId">The item Id to update.</param>
    /// <param name="containerKey">The key used to resolve the Cosmos DB container.</param>
    /// <param name="partitionKeyValue">The value of the partition key.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated item.</returns>
    public Task<TItem> ReplaceItemAsync<TItem>(
        TItem item,
        string itemId,
        string containerKey,
        string partitionKeyValue,
        CancellationToken cancellationToken = default) where TItem : class;

    /// <summary>
    /// Replaces an item using a <see cref="PartitionKey"/>.
    /// </summary>
    /// <typeparam name="TItem">The type of the item to update.</typeparam>
    /// <param name="item">The item to update.</param>
    /// <param name="itemId">The item Id to update.</param>
    /// <param name="containerKey">The key used to resolve the Cosmos DB container.</param>
    /// <param name="partitionKey">The partition key for the item.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated item.</returns>
    public Task<TItem> ReplaceItemAsync<TItem>(
        TItem item,
        string itemId,
        string containerKey,
        PartitionKey partitionKey,
        CancellationToken cancellationToken = default) where TItem : class;

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
    CancellationToken cancellationToken = default) where TItem : class;

    /// <summary>
    /// Deletes an item using a <see cref="PartitionKey"/>.
    /// </summary>
    /// <typeparam name="TItem">The type of the item to delete.</typeparam>
    /// <param name="id">The ID of the item to delete.</param>
    /// <param name="containerKey">The key used to resolve the Cosmos DB container.</param>
    /// <param name="partitionKey">The partition key for the item.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    public Task DeleteItemAsync<TItem>(
        string id,
        string containerKey,
        PartitionKey partitionKey,
        CancellationToken cancellationToken = default) where TItem : class;
}
