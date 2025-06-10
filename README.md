# Cosmos DB Persistence Library

## Overview
The **Cosmos DB Persistence Library** provides a structured abstraction for interacting with **Azure Cosmos DB**. It includes **command handlers**, **query handlers**, **pagination utilities**, and **container providers** to streamline data operations.

## Features
- **CRUD Operations**: Create, update, replace, and delete items efficiently.
- **Optimized Queries**: Supports **point reads**, **SQL-based queries**, and **LINQ expressions**.
- **Pagination Support**: Fetch large datasets in manageable pages.
- **Encapsulated Client Access**: Centralized management of Cosmos DB containers and client interactions.
- **Efficient Data Streaming**: Convert LINQ queries into `FeedIterator` for optimized batch processing.

## Installation
**Dependency Registration (Composition Root)**
Register Cosmos DB dependencies in your application's startup configuration:
```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

var serviceCollection = new ServiceCollection();
serviceCollection.AddCosmosDbDependencies();
var serviceProvider = serviceCollection.BuildServiceProvider();
```

## Example Usage
**Command Handler (Create, Update, Delete)**

```csharp
using Microsoft.Azure.Cosmos;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Handlers.Command;

ICosmosDbCommandHandler commandHandler = new CosmosDbCommandHandler(containerProvider);

// Create an item
var item = new MyItem { Id = "123", Name = "Sample Item" };
await commandHandler.CreateItemAsync(item, "myContainer", "partitionKeyValue");
// Update or insert an item
await commandHandler.UpsertItemAsync(item, "myContainer", "partitionKeyValue");
// Replace an item
await commandHandler.ReplaceItemAsync(item, "123", "myContainer", "partitionKeyValue");
// Delete an item
await commandHandler.DeleteItemAsync<MyItem>("123", "myContainer", "partitionKeyValue");
```

**Query Handler (Point Reads & SQL Queries)**

```csharp
using Microsoft.Azure.Cosmos;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Handlers.Query;

ICosmosDbQueryHandler queryHandler = new CosmosDbQueryHandler(containerProvider);

// Perform a point read (single-item lookup)
var item = await queryHandler.ReadItemByIdAsync<MyItem>(
    id: "123",
    containerKey: "myContainer",
    partitionKeyValue: "partitionKeyValue");

// Query using SQL
string sqlQuery = "SELECT * FROM c WHERE c.Category = 'Electronics'";
IEnumerable<MyItem> items = await queryHandler.ReadItemsAsync<MyItem>("myContainer", sqlQuery);
```

**LINQ-Based Query**

```csharp
Expression<Func<MyItem, MyItem>> selector = item => new MyItem { Id = item.Id, Name = item.Name };
Expression<Func<MyItem, bool>> predicate = item => item.Category == "Electronics";

IEnumerable<MyItem> items = await queryHandler.ReadItemsAsync("myContainer", selector, predicate);
```

**Paginated Query Handler**

```csharp
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Handlers.Query;

IPaginatedCosmosDbQueryHandler paginatedQueryHandler = new PaginatedCosmosDbQueryHandler(containerProvider);

IEnumerable<MyItem> pagedItems = await paginatedQueryHandler.ReadPaginatedItemsAsync(
    "myContainer", selector, predicate, pageNumber: 1, pageSize: 20);

// Get total count of matching items
int itemCount = await paginatedQueryHandler.GetItemCountAsync<MyItem>("myContainer", predicate);
```

**LINQ to FeedIterator Conversion**

```csharp
using Microsoft.Azure.Cosmos;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Handlers.Query;

IQueryableToFeedIterator converter = new QueryableToFeedIterator();

IQueryable<MyItem> query = cosmosContainer.GetItemLinqQueryable<MyItem>()
    .Where(item => item.Category == "Electronics");

FeedIterator<MyItem> iterator = converter.GetFeedIterator(query);
```

**Container Provider (Retrieve Cosmos DB Containers)**

```csharp
using Microsoft.Azure.Cosmos;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Providers;

ICosmosDbContainerProvider containerProvider = new CosmosDbContainerProvider();
Container container = await containerProvider.GetContainerAsync("myContainer");
```

**Client Provider (Execute Cosmos DB Operations)**

```csharp
using Microsoft.Azure.Cosmos;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Providers;

ICosmosDbClientProvider clientProvider = new CosmosDbClientProvider();

var database = await clientProvider.InvokeCosmosClientAsync(async client =>
{
    return await client.CreateDatabaseIfNotExistsAsync("MyDatabase");
});
```

## Dependencies
- Microsoft.Azure.Cosmos
- .NET 6 or later recommended