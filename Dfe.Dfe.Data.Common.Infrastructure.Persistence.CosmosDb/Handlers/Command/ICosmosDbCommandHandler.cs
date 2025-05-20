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
    /// Creates multiple items in the specified container.
    /// </summary>
    /// <typeparam name="TItem">The type of the items to create.</typeparam>
    /// <param name="items">The collection of items to create.</param>
    /// <param name="containerKey">The key used to resolve the Cosmos DB container.</param>
    /// <param name="partitionKeyValue">The value of the partition key.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A collection of the created items.</returns>
    public Task<IEnumerable<TItem>> CreateItemsAsync<TItem>(
        IEnumerable<TItem> items,
        string containerKey,
        string partitionKeyValue,
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
    public Task<TItem> UpdateItemAsync<TItem>(
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
    public Task<TItem> UpdateItemAsync<TItem>(
        TItem item,
        string containerKey,
        PartitionKey partitionKey,
        CancellationToken cancellationToken = default) where TItem : class;

    /// <summary>
    /// Updates or inserts multiple items in the specified container.
    /// </summary>
    /// <typeparam name="TItem">The type of the items to update.</typeparam>
    /// <param name="items">The collection of items to update.</param>
    /// <param name="containerKey">The key used to resolve the Cosmos DB container.</param>
    /// <param name="partitionKeyValue">The value of the partition key.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A collection of the updated items.</returns>
    public Task<IEnumerable<TItem>> UpdateItemsAsync<TItem>(
        IEnumerable<TItem> items,
        string containerKey,
        string partitionKeyValue,
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
