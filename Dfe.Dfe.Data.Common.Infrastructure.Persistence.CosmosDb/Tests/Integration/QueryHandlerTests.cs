using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Handlers.Query;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration.Fixture;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration.Fixture.Model;
using FluentAssertions;
using System.Linq.Expressions;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration;

[Collection(QueryIntegrationTestCollection.Name)]
public sealed class CosmosDbQueryHandlerTests
{
    private readonly CosmosDbHandlerFixture<ICosmosDbQueryHandler> _fixture;

    public CosmosDbQueryHandlerTests(CosmosDbHandlerFixture<ICosmosDbQueryHandler> fixture)
    {
        _fixture = fixture;
        ContainerName = fixture.ContainerName;
    }

    private string ContainerName { get; }

    [Fact]
    public async Task ReadItemByIdAsync_ContainerRecordsAndValidQuery_ReturnsCorrectResult()
    {
        // arrange
        IEnumerable<ContainerRecord>? allItems =
            await _fixture.Handler.ReadItemsAsync<ContainerRecord>(
                containerKey: ContainerName,
                query: "SELECT * FROM c");

        if (!allItems.Any()){
            Assert.Fail($"{ContainerName} does not contain any items to read by ID.");
        }

        var itemId = allItems.First().id;

        // act
        ContainerRecord? result =
            await _fixture.Handler.ReadItemByIdAsync<ContainerRecord>(
                id: itemId,
                containerKey: ContainerName,
                partitionKeyValue: itemId);

        // assert
        result.Should().NotBeNull()
            .And.BeAssignableTo<ContainerRecord>();

        await Task.Delay(1000);
    }

    [Fact]
    public async Task ReadItemsAsync_WithLambda_ContainerRecordsAndValidQuery_ReturnsCorrectResults()
    {
        // arrange
        IEnumerable<ContainerRecord> allItems =
            await _fixture.Handler.ReadItemsAsync<ContainerRecord>(
                containerKey: ContainerName,
                query: "SELECT * FROM c");

        var itemId = allItems.First().id;

        Expression<Func<ContainerRecord, bool>> predicate = item => item.id == itemId;
        Expression<Func<ContainerRecord, ContainerRecord>> selector = item => new ContainerRecord { id = item.id };

        // act
        IEnumerable<ContainerRecord> results =
            await _fixture.Handler.ReadItemsAsync(
                containerKey: ContainerName,
                selector: selector,
                predicate: predicate);

        // assert
        results.Should().NotBeNullOrEmpty()
            .And.HaveCount(1)
            .And.AllBeAssignableTo<ContainerRecord>();

        await Task.Delay(1000);
    }

    [Fact]
    public async Task ReadItemsAsync_ContainerRecordsAndValidQuery_ReturnsCorrectResults()
    {
        IEnumerable<ContainerRecord> results =
            await _fixture.Handler.ReadItemsAsync<ContainerRecord>(
                containerKey: ContainerName,
                query: "SELECT * FROM c");

        results.Should().NotBeNullOrEmpty()
            .And.HaveCountGreaterThan(100)
            .And.AllBeAssignableTo<ContainerRecord>();

        await Task.Delay(1000);
    }
}