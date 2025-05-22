using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Handlers.Query;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration.Fixture;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration.Fixture.Model;
using FluentAssertions;
using System.Linq.Expressions;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration;

public sealed class QueryHandlerTests
{
    [Fact]
    public async Task ReadItemsAsync_ContainerRecordsAndValidQuery_ReturnsCorrectResults()
    {
        var (queryHandler, context, _) =
            await new CosmosDbTestHelper().CreateIsolatedHandlerAsync<ICosmosDbQueryHandler>();

        try
        {
            var results = await queryHandler.ReadItemsAsync<ContainerRecord>(
                containerKey: "test-container",
                query: "SELECT TOP 10 * FROM c");

            results.Should().NotBeNullOrEmpty()
                .And.HaveCount(10)
                .And.AllBeAssignableTo<ContainerRecord>();
        }
        finally
        {
            context.CleanUpResources();
        }
    }

    [Fact]
    public async Task ReadItemByIdAsync_ContainerRecordsAndValidQuery_ReturnsCorrectResult()
    {
        var (queryHandler, context, records) =
            await new CosmosDbTestHelper().CreateIsolatedHandlerAsync<ICosmosDbQueryHandler>();

        try
        {
            var itemId = records.First().id;

            var result = await queryHandler.ReadItemByIdAsync<ContainerRecord>(
                id: itemId,
                containerKey: "test-container",
                partitionKeyValue: itemId);

            result.Should().NotBeNull()
                .And.BeAssignableTo<ContainerRecord>();
        }
        finally
        {
            context.CleanUpResources();
        }
    }

    [Fact]
    public async Task ReadItemsAsync_WithLambda_ContainerRecordsAndValidQuery_ReturnsCorrectResults()
    {
        var (queryHandler, context, records) =
            await new CosmosDbTestHelper().CreateIsolatedHandlerAsync<ICosmosDbQueryHandler>();

        try
        {
            var itemId = records.First().id;

            Expression<Func<ContainerRecord, bool>> predicate = item => item.id.Contains(itemId);
            Expression<Func<ContainerRecord, ContainerRecord>> selector = _ => new ContainerRecord { id = itemId };

            var results = await queryHandler.ReadItemsAsync<ContainerRecord>(
                containerKey: "test-container",
                selector: selector,
                predicate: predicate);

            results.Should().NotBeNullOrEmpty()
                .And.HaveCount(1)
                .And.AllBeAssignableTo<ContainerRecord>();
        }
        finally
        {
            context.CleanUpResources();
        }
    }
}
