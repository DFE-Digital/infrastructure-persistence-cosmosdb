using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Handlers.Query;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Providers;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Unit.Handlers.Query.TestDoubles;
using Microsoft.Azure.Cosmos;
using Moq;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Unit.Handlers.Query;

public class PaginatedCosmosDbQueryHandlerTests
{
    [Fact]
    public async Task ReadPaginatedItemsAsync_ShouldReturnPaginatedResults()
    {
        // Arrange
        IQueryable<TestItem> testItems = new List<TestItem>{
                new() { Id = "1", Name = "TestItem1" },
                new() { Id = "2", Name = "TestItem2" }
            }
            .AsQueryable();

        // Convert IQueryable to IOrderedQueryable using OrderBy
        IOrderedQueryable<TestItem> orderedTestItems =
            testItems.OrderBy(item => item.Id);
        Mock<FeedIterator<TestItem>> mockFeedIterator =
            FeedIteratorTestDouble.MockFor(orderedTestItems);
        Mock<Container> mockContainer =
            CosmosContainerTestDouble.MockFor(orderedTestItems);
        Mock<ICosmosDbContainerProvider> mockContainerProvider =
            ContainerProviderTestDouble.MockFor(mockContainer.Object);
        Mock<IQueryableToFeedIterator> mockQueryableToFeedIterator =
            QueryableToFeedIteratorTestDouble.MockFor(mockFeedIterator.Object);

        var handler =
            new PaginatedCosmosDbQueryHandler(
                mockQueryableToFeedIterator.Object, mockContainerProvider.Object);

        // Act
        var result =
            await handler.ReadPaginatedItemsAsync<TestItem>(
                "containerKey",
                item => item,
                item => item.Id == "1",
                pageNumber: 1, pageSize: 2, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count());
        //Assert.Equal(testData.Take(2), result);
    }

    public class TestItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}