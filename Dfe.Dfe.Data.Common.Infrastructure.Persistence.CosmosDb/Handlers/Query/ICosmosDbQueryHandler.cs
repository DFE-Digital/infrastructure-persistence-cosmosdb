using Microsoft.Azure.Cosmos;
using System.Linq.Expressions;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Handlers.Query;

/// <summary>
/// Provides an abstraction for querying cosmos db container items either using point reads
/// (i.e. key/value lookup on a single item ID and partition key), see:
/// <a href="https://devblogs.microsoft.com/cosmosdb/point-reads-versus-queries/">
/// Point reads vs queries in cosmos db</a>
/// or via <b>Structured Query Language (SQL)</b>, see:
/// <a href="https://cosmosdb.github.io/labs/dotnet/labs/03-querying_in_azure_cosmosdb.html">
/// Querying in azure cosmos db</a>
/// </summary>
public interface ICosmosDbQueryHandler
{
    /// <summary>
    /// Performs a point-read for an item within the specified Azure Cosmos db container, as an asynchronous operation.
    /// The point read is a key/value lookup on a single item ID and partition key, and offers increased performance
    /// and reduced costs per RU in comparison to the more traditional query-led approach.
    /// </summary>
    /// <typeparam name="TItem">
    /// The type of resource to return in the <see cref="ItemResponse{TItem}"/>.
    /// </typeparam>
    /// <param name="id">
    /// The unique id of the resource item to be extracted from the container.
    /// </param>
    /// <param name="containerKey">
    /// Represents a container key value in the Azure Cosmos DB service.
    /// </param>
    /// <param name="partitionKeyValue">
    /// Represents a partition key value in the Azure Cosmos DB service.
    /// </param>
    /// <param name="cancellationToken">
    /// The notification that is Propagated when the read operation should be canceled.
    /// </param>
    /// <returns>
    /// A configured instance of the specified generic type which encapsulates the
    /// requested cosmos db record item.
    /// </returns>
    Task<TItem> ReadItemByIdAsync<TItem>(
       string id,
       string containerKey,
       string partitionKeyValue,
       CancellationToken cancellationToken = default) where TItem : class;

    /// <summary>
    /// Performs a query using the Structured Query Language (SQL), the definition of which
    /// should be provisioned in the query string value provided.
    /// </summary>
    /// <typeparam name="TItem">
    /// The type of resource to return in the <see cref="ItemResponse{TItem}"/>.
    /// </typeparam>
    /// <param name="containerKey">
    /// Represents a container key value in the Azure Cosmos DB service.
    /// </param>
    /// <param name="query">
    /// The string value that encapsulates the Structured Query Language (SQL) query definition,
    /// see: <a href="https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/query/">cosmos db query definitions</a>
    /// </param>
    /// <param name="cancellationToken">
    /// The notification that is Propagated when the read operation should be canceled.
    /// </param>
    /// <returns>
    /// A configured instance of the specified generic type which encapsulates the
    /// requested cosmos db record item.
    /// </returns>
    Task<IEnumerable<TItem>> ReadItemsAsync<TItem>(
        string containerKey,
        string query,
        CancellationToken cancellationToken = default) where TItem : class;

    /// <summary>
    /// Performs a query by creating a LINQ query for items under a container, utilising the
    /// lambda expressions provisioned for the selector (projects each element of the specified
    /// sequence into a new form), and predicate (filters a sequence of values based on the
    /// criteria specified). The Azure Cosmos DB LINQ provider compiles LINQ to SQL statements.
    /// Refer to https://docs.microsoft.com/azure/cosmos-db/sql-query-linq-to-sql for the list of
    /// expressions supported by the Azure Cosmos DB LINQ provider.
    /// </summary>
    /// <typeparam name="TItem">
    /// The type of resource to return in the <see cref="ItemResponse{TItem}"/>.
    /// </typeparam>
    /// <param name="containerKey">
    /// Represents a container key value in the Azure Cosmos DB service.
    /// </param>
    /// <param name="selector">
    /// Lambda expression used to define what elements to project to the specified sequence.
    /// </param>
    /// <param name="predicate">
    /// Lambda expression used to define the predicates on which to filter a sequence of values
    /// based on the criteria specified.
    /// </param>
    /// <param name="cancellationToken">
    /// The notification that is Propagated when the read operation should be canceled.
    /// </param>
    /// <returns>
    /// A configured instance of the specified generic type which encapsulates the
    /// requested cosmos db record item.
    /// </returns>
    Task<IEnumerable<TItem>> ReadItemsAsync<TItem>(
        string containerKey,
        Expression<Func<TItem, TItem>> selector,
        Expression<Func<TItem, bool>> predicate,
        CancellationToken cancellationToken = default) where TItem : class;

    /// <summary>
    /// Reads all items from the provisioned <see cref="FeedIterator{TItem}"/> until the results are fully drained.
    /// </summary>
    /// <typeparam name="TItem">
    /// The type of resource to return in the <see cref="ItemResponse{TItem}"/>.
    /// </typeparam>
    /// <param name="feedIterator">
    /// Cosmos Result set iterator that keeps track of the continuation token when retrieving results form a query.
    /// </param>
    /// <param name="cancellationToken">
    /// The notification that is Propagated when the read operation should be canceled.
    /// </param>
    /// <returns>
    /// A configured instance of the specified generic type which encapsulates the
    /// requested cosmos db record item.
    /// </returns>
    Task<IEnumerable<TItem>> ReadItemsAsync<TItem>(
        FeedIterator<TItem> feedIterator,
        CancellationToken cancellationToken = default) where TItem : class;
}