using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Handlers.Command;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Handlers.Query;
using System.Runtime.CompilerServices;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration.Fixture;

/// <summary>
/// Defines a test collection for Cosmos DB integration tests that use a command handler.
/// This ensures that multiple test classes using <see cref="CosmosDbHandlerFixture{ICosmosDbCommandHandler}"/>
/// share the same fixture instance, avoiding redundant setup.
/// </summary>
[CollectionDefinition(CommandIntegrationTestCollection.Name)] // Groups tests using command handlers
public class CommandIntegrationTestCollection :
    ICollectionFixture<CosmosDbHandlerFixture<ICosmosDbCommandHandler>>
{
    /// <summary>
    /// Establishes the name of the collection for command integration tests.
    /// </summary>
    public const string Name = "CommandIntegrationTestCollection";
}

/// <summary>
/// Defines a test collection for Cosmos DB integration tests that use a query handler.
/// This ensures that multiple test classes using <see cref="CosmosDbHandlerFixture{ICosmosDbQueryHandler}"/>
/// share the same fixture instance, avoiding redundant setup.
/// </summary>
[CollectionDefinition(QueryIntegrationTestCollection.Name)] // Groups tests using query handlers
public class QueryIntegrationTestCollection : // Uses query handler fixture
    ICollectionFixture<CosmosDbHandlerFixture<ICosmosDbQueryHandler>>
{
    /// <summary>
    /// Establishes the name of the collection for query integration tests.
    /// </summary>
    public const string Name = "QueryIntegrationTestCollection";
}
