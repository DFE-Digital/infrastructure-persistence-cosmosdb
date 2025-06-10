using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Handlers.Query;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Providers;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Unit.Handlers.Query.TestDoubles;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Unit.Handlers.Query.TestDoubles.Shared;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Unit.Handlers.TestDoubles;
using FluentAssertions;
using Microsoft.Azure.Cosmos;
using Moq;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Unit.Handlers.Query;

public class PaginatedCosmosDbQueryHandlerTests
{
    [Fact]
    public async Task ReadPaginatedItemsAsync_ShouldReturnPaginatedResults()
    {
        // Arrange
        IQueryable<TestResponseObject> testItems =
            TestResponseObjects.Create(totalCount: 10).AsQueryable();
        // Convert IQueryable to IOrderedQueryable using OrderBy
        IOrderedQueryable<TestResponseObject> orderedTestItems =
            testItems.OrderBy(item => item.Id);
        Mock<FeedIterator<TestResponseObject>> mockFeedIterator =
            FeedIteratorTestDouble.MockFor(orderedTestItems);
        Mock<Container> mockContainer =
            CosmosContainerTestDouble.MockFor(orderedTestItems);
        Mock<ICosmosDbContainerProvider> mockContainerProvider =
            ContainerProviderTestDouble.MockFor(mockContainer.Object);
        Mock<IQueryableToFeedIterator> mockQueryableToFeedIterator =
            QueryableToFeedIteratorTestDouble.MockFor(mockFeedIterator.Object);

        PaginatedCosmosDbQueryHandler handler =
            new(mockQueryableToFeedIterator.Object, mockContainerProvider.Object);

        // Act
        IEnumerable<TestResponseObject> result =
            await handler.ReadPaginatedItemsAsync<TestResponseObject>(
                "containerKey",
                item => item,
                item => item.Id == "1",
                pageNumber: 1, pageSize: 2, CancellationToken.None);

        // Assert - we'll just check the values even though they're mocked to ensure
        // the right depth of calls is being made etc although we'll still verify these calls.
        Assert.NotNull(result);
        Assert.NotEmpty(result);

        // Ensure the correct single call was made to the mocked feed iterator.
        mockFeedIterator.Verify(feedIterator =>
            feedIterator.ReadNextAsync(It.IsAny<CancellationToken>()), Times.Once);

        // Ensure the correct single call was made to the mocked container.
        mockContainer.Verify(container =>
            container.ReadItemAsync<TestResponseObject>(
                It.IsAny<string>(), It.IsAny<PartitionKey>(), default, default), Times.Never);

        // Ensure the correct single call was made to the mocked container provider.
        mockContainerProvider.Verify(containerProvider =>
            containerProvider.GetContainerAsync("containerKey"), Times.Once);
    }

    [Fact]
    public async Task GetItemCountAsync_ShouldReturnResultsCount()
    {
        // Arrange
        IQueryable<TestResponseObject> testItems =
            TestResponseObjects.Create(totalCount: 10).AsQueryable();
        // Convert IQueryable to IOrderedQueryable using OrderBy
        IOrderedQueryable<TestResponseObject> orderedTestItems =
            testItems.OrderBy(item => item.Id);
        Mock<FeedIterator<TestResponseObject>> mockFeedIterator =
            FeedIteratorTestDouble.MockFor(orderedTestItems);
        Mock<Container> mockContainer =
            CosmosContainerTestDouble.MockFor(orderedTestItems);
        Mock<ICosmosDbContainerProvider> mockContainerProvider =
            ContainerProviderTestDouble.MockFor(mockContainer.Object);
        Mock<IQueryableToFeedIterator> mockQueryableToFeedIterator =
            QueryableToFeedIteratorTestDouble.MockFor(mockFeedIterator.Object);

        PaginatedCosmosDbQueryHandler handler =
            new(mockQueryableToFeedIterator.Object, mockContainerProvider.Object);

        // Act
        int result =
            await handler.GetItemCountAsync<TestResponseObject>(
                containerKey: "containerKey",
                item => item.Id == "1" || item.Id == "2",
                CancellationToken.None);

        Assert.Equal(2, result);

        // Ensure the correct single call was made to the mocked feed iterator.
        mockFeedIterator.Verify(feedIterator =>
            feedIterator.ReadNextAsync(It.IsAny<CancellationToken>()), Times.Never);

        // Ensure the correct single call was made to the mocked container.
        mockContainer.Verify(container =>
            container.ReadItemAsync<TestResponseObject>(
                It.IsAny<string>(), It.IsAny<PartitionKey>(), default, default), Times.Never);

        // Ensure the correct single call was made to the mocked container provider.
        mockContainerProvider.Verify(containerProvider =>
            containerProvider.GetContainerAsync("containerKey"), Times.Once);
    }

        [Fact]
    public void Constructor_WithNullContainerProvider_ThrowsExpectedArgumentNullException()
    {
        // Arrange.
        Mock<IQueryableToFeedIterator> mockQueryableToFeedIterator =
            QueryableToFeedIteratorTestDouble.MockFor(
                FeedIteratorTestDouble.DefaultMock<TestResponseObject>().Object);

        // Act
        Action failedAction = () =>
            new PaginatedCosmosDbQueryHandler(
                mockQueryableToFeedIterator.Object, cosmosDbContainerProvider: null!);

        // Assert
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(failedAction);

        exception.Message.Should().Be("Value cannot be null. (Parameter 'cosmosDbContainerProvider')");
    }

    [Fact]
    public void Constructor_WithNullQueryableToFeedIterator_ThrowsExpectedArgumentNullException()
    {
        // Arrange.
        IQueryable<TestResponseObject> testItems =
            TestResponseObjects.Create(totalCount: 10).AsQueryable();
        // Convert IQueryable to IOrderedQueryable using OrderBy
        IOrderedQueryable<TestResponseObject> orderedTestItems =
            testItems.OrderBy(item => item.Id);
        Mock<Container> mockContainer =
            CosmosContainerTestDouble.MockFor(orderedTestItems);
        Mock<ICosmosDbContainerProvider> mockContainerProvider =
            ContainerProviderTestDouble.MockFor(mockContainer.Object);

        // Act
        Action failedAction = () =>
            new PaginatedCosmosDbQueryHandler(
                queryableToFeedIterator: null!, mockContainerProvider.Object);

        // Assert
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(failedAction);

        exception.Message.Should().Be("Value cannot be null. (Parameter 'queryableToFeedIterator')");
    }

    [Theory]
    [InlineData(null!, typeof(ArgumentNullException))]
    [InlineData("", typeof(ArgumentException))]
    public Task ReadItemByIdAsync_WithNullOrEmptyContainerKey_ThrowsExpectedArgumentExceptionType(
        string containerKey, Type expectedException)
    {
        // Arrange  
        PaginatedCosmosDbQueryHandler paginatedCosmosDbQueryHandler = CreateDefaultPaginatedCosmosDbQueryHandler();

        // Act, assert 
        return Assert.ThrowsAsync(expectedException, () =>
             paginatedCosmosDbQueryHandler.ReadPaginatedItemsAsync<TestResponseObject>(
                containerKey,
                item => item,
                item => item.Id == "1",
                pageNumber: 1, pageSize: 2, CancellationToken.None));
    }

    [Fact]
    public Task ReadItemByIdAsync_WithNullSelector_ThrowsExpectedArgumentNullException()
    {
        // Arrange 
        PaginatedCosmosDbQueryHandler queryHandler = CreateDefaultPaginatedCosmosDbQueryHandler();

        // Act, assert 
        return Assert.ThrowsAsync<ArgumentNullException>(() =>
             queryHandler.ReadPaginatedItemsAsync<TestResponseObject>(
                containerKey: "containerKey",
                selector: null!,
                item => item.Id == "1",
                pageNumber: 1, pageSize: 2, CancellationToken.None));
    }

    [Fact]
    public Task ReadItemByIdAsync_WithNullPredicate_ThrowsExpectedArgumentNullException()
    {
        // Arrange 
        PaginatedCosmosDbQueryHandler queryHandler = CreateDefaultPaginatedCosmosDbQueryHandler();

        // Act, assert 
        return Assert.ThrowsAsync<ArgumentNullException>(() =>
             queryHandler.ReadPaginatedItemsAsync<TestResponseObject>(
                containerKey: "containerKey",
                item => item,
                predicate: null!,
                pageNumber: 1, pageSize: 2, CancellationToken.None));
    }

    [Fact]
    public Task ReadItemByIdAsync_WithPageNumberLessThanOne_ThrowsExpectedArgumentException()
    {
        // Arrange 
        PaginatedCosmosDbQueryHandler queryHandler = CreateDefaultPaginatedCosmosDbQueryHandler();

        // Act, assert 
        return Assert.ThrowsAsync<ArgumentException>(
            paramName: "pageNumber", () =>
             queryHandler.ReadPaginatedItemsAsync<TestResponseObject>(
                containerKey: "containerKey",
                item => item,
                item => item.Id == "1",
                pageNumber: 0, pageSize: 2, CancellationToken.None));
    }

    [Fact]
    public Task ReadItemByIdAsync_WithPageSizeLessThanOne_ThrowsExpectedArgumentException()
    {
        // Arrange 
        PaginatedCosmosDbQueryHandler queryHandler = CreateDefaultPaginatedCosmosDbQueryHandler();

        // Act, assert 
        return Assert.ThrowsAsync<ArgumentException>(
            paramName: "pageSize", () =>
             queryHandler.ReadPaginatedItemsAsync<TestResponseObject>(
                containerKey: "containerKey",
                item => item,
                item => item.Id == "1",
                pageNumber: 1, pageSize: 0, CancellationToken.None));
    }

    /// <summary>
    /// Creates a default instance of <see cref="CosmosDbQueryHandler"/> for testing purposes.
    /// </summary>
    /// <returns></returns>
    private static PaginatedCosmosDbQueryHandler CreateDefaultPaginatedCosmosDbQueryHandler()
    {
        ItemResponse<TestResponseObject> mockItemResponse =
            ItemResponseTestDouble.MockFor<TestResponseObject>(null!);
        Mock<Container> mockContainer =
            CosmosContainerTestDouble.MockFor(mockItemResponse);
        Mock<ICosmosDbContainerProvider> mockContainerProvider =
            ContainerProviderTestDouble.MockFor(mockContainer.Object);
        Mock<IQueryableToFeedIterator> mockQueryableToFeedIterator =
            QueryableToFeedIteratorTestDouble.MockFor(
                FeedIteratorTestDouble.DefaultMock<TestResponseObject>().Object);

        return new PaginatedCosmosDbQueryHandler(
            mockQueryableToFeedIterator.Object, mockContainerProvider.Object);
    }
}