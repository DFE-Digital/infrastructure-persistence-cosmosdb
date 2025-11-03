using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Handlers.Query;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Providers;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Unit.Handlers.Query.TestDoubles;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Unit.Handlers.Query.TestDoubles.Shared;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Unit.Handlers.TestDoubles;
using FluentAssertions;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Serialization.HybridRow;
using Moq;
using System.Linq.Expressions;
using System.Net;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Unit.Handlers.Query;

public sealed class CosmosDbQueryHandlerTests
{
    [Fact]
    public async Task TryReadItemByIdAsync_Returns_Null_When_CosmosExceptionNotFound_Thrown()
    {
        // Arrange
        Mock<Container> mockContainer =
            CosmosContainerTestDouble.DefaultMock();

        mockContainer
            .Setup((container) => container.ReadItemAsync<TestResponseObject>(
                It.IsAny<string>(), 
                It.IsAny<PartitionKey>(), 
                default, 
                default))
            .ReturnsAsync(() => throw CosmosExceptionTestDoubles.WithStatusCode(HttpStatusCode.NotFound));

        Mock<ICosmosDbContainerProvider> mockContainerProvider =
            ContainerProviderTestDouble.MockFor(mockContainer.Object);
        
        Mock<IQueryableToFeedIterator> mockQueryableToFeedIterator =
            QueryableToFeedIteratorTestDouble.MockFor(
                FeedIteratorTestDouble.DefaultMock<TestResponseObject>().Object);

        CosmosDbQueryHandler queryHandler =
            new(mockQueryableToFeedIterator.Object, mockContainerProvider.Object);

        // Act
        TestResponseObject? result =
            await queryHandler.TryReadItemByIdAsync<TestResponseObject>("1", "containerKey", "partitionKey");

        // Assert
        Assert.Null(result);

        // Ensure the correct two calls were made to the mocked container.
        mockContainer.Verify(container =>
            container.ReadItemAsync<TestResponseObject>(
                It.IsAny<string>(), It.IsAny<PartitionKey>(), default, default), Times.Once);

        // Ensure the correct single call was made to the mocked container provider.
        mockContainerProvider.Verify(containerProvider =>
            containerProvider.GetContainerAsync("containerKey"), Times.Once);
    }

    [Fact]
    public async Task TryReadItemByIdAsync_Throws_When_CosmosException_ThatIsNot_NotFound()
    {
        // Arrange
        Mock<Container> mockContainer =
            CosmosContainerTestDouble.DefaultMock();

        mockContainer
            .Setup((container) => container.ReadItemAsync<TestResponseObject>(
                It.IsAny<string>(), 
                It.IsAny<PartitionKey>(), 
                default, 
                default))
            .ReturnsAsync(() => throw CosmosExceptionTestDoubles.WithStatusCode(HttpStatusCode.OK));

        Mock<ICosmosDbContainerProvider> mockContainerProvider =
            ContainerProviderTestDouble.MockFor(mockContainer.Object);

        Mock<IQueryableToFeedIterator> mockQueryableToFeedIterator =
            QueryableToFeedIteratorTestDouble.MockFor(
                FeedIteratorTestDouble.DefaultMock<TestResponseObject>().Object);

        CosmosDbQueryHandler queryHandler =
            new(mockQueryableToFeedIterator.Object, mockContainerProvider.Object);

        // Act
        Func<Task<TestResponseObject?>> handle = () => queryHandler.TryReadItemByIdAsync<TestResponseObject>("1", "containerKey", "partitionKey");

        // Assert
        await Assert.ThrowsAsync<CosmosException>(handle);

        // Ensure the correct two calls were made to the mocked container.
        mockContainer.Verify(container =>
            container.ReadItemAsync<TestResponseObject>(
                It.IsAny<string>(), It.IsAny<PartitionKey>(), default, default), Times.Once);

        // Ensure the correct single call was made to the mocked container provider.
        mockContainerProvider.Verify(containerProvider =>
            containerProvider.GetContainerAsync("containerKey"), Times.Once);
    }

    [Fact]
    public async Task TryReadItemByIdAsync_Returns_ExpectedItem()
    {
        // Arrange
        ItemResponse<TestResponseObject> mockItemResponse =
            ItemResponseTestDouble.MockFor(TestResponseObject.Create(id: "1", name: "TestItem"));

        Mock<Container> mockContainer =
            CosmosContainerTestDouble.MockFor(mockItemResponse);

        Mock<ICosmosDbContainerProvider> mockContainerProvider =
            ContainerProviderTestDouble.MockFor(mockContainer.Object);

        Mock<IQueryableToFeedIterator> mockQueryableToFeedIterator =
            QueryableToFeedIteratorTestDouble.MockFor(
                FeedIteratorTestDouble.DefaultMock<TestResponseObject>().Object);

        CosmosDbQueryHandler queryHandler =
            new(mockQueryableToFeedIterator.Object, mockContainerProvider.Object);

        // Act
        TestResponseObject? result = await queryHandler.TryReadItemByIdAsync<TestResponseObject>("1", "containerKey", "partitionKey");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("1", result.Id);
        Assert.Equal("TestItem", result.Name);

        // Ensure the correct two calls were made to the mocked container.
        mockContainer.Verify(container =>
            container.ReadItemAsync<TestResponseObject>(
                It.IsAny<string>(), It.IsAny<PartitionKey>(), default, default), Times.Once);

        // Ensure the correct single call was made to the mocked container provider.
        mockContainerProvider.Verify(containerProvider =>
            containerProvider.GetContainerAsync("containerKey"), Times.Once);
    }

    [Fact]
    public async Task ReadItemByIdAsync_ReturnsExpectedItem()
    {
        // Arrange
        ItemResponse<TestResponseObject>  mockItemResponse =
            ItemResponseTestDouble.MockFor(TestResponseObject.Create(id: "1", name: "TestItem"));
        Mock<Container> mockContainer =
            CosmosContainerTestDouble.MockFor(mockItemResponse);
        Mock<ICosmosDbContainerProvider> mockContainerProvider =
            ContainerProviderTestDouble.MockFor(mockContainer.Object);
        Mock<IQueryableToFeedIterator> mockQueryableToFeedIterator =
            QueryableToFeedIteratorTestDouble.MockFor(
                FeedIteratorTestDouble.DefaultMock<TestResponseObject>().Object);

        CosmosDbQueryHandler queryHandler =
            new(mockQueryableToFeedIterator.Object, mockContainerProvider.Object);

        // Act
        TestResponseObject result =
            await queryHandler.ReadItemByIdAsync<TestResponseObject>("1", "containerKey", "partitionKey");

        // Assert - we'll just check the values even though they're mocked to ensure
        // the right depth of calls is being made etc although we'll still verify these calls.
        Assert.NotNull(result);
        Assert.Equal("1", result.Id);
        Assert.Equal("TestItem", result.Name);

        // Ensure the correct two calls were made to the mocked container.
        mockContainer.Verify(container =>
            container.ReadItemAsync<TestResponseObject>(
                It.IsAny<string>(), It.IsAny<PartitionKey>(), default, default), Times.Once);

        // Ensure the correct single call was made to the mocked container provider.
        mockContainerProvider.Verify(containerProvider =>
            containerProvider.GetContainerAsync("containerKey"), Times.Once);
    }

    [Fact]
    public async Task ReadItemsAsync_ReturnsExpectedItems()
    {
        // Arrange
        IQueryable<TestResponseObject> testItems =
            TestResponseObjects.Create(totalCount:10).AsQueryable();

        // Convert IQueryable to IOrderedQueryable using OrderBy
        IOrderedQueryable<TestResponseObject> orderedTestItems =
            testItems.OrderBy(item => item.Id);
        Mock<FeedIterator<TestResponseObject>> mockFeedIterator =
            FeedIteratorTestDouble.MockFor(orderedTestItems);
        Mock<Container> mockContainer =
            CosmosContainerTestDouble.MockFor(mockFeedIterator.Object);
        Mock<ICosmosDbContainerProvider> mockContainerProvider =
            ContainerProviderTestDouble.MockFor(mockContainer.Object);
        Mock<IQueryableToFeedIterator> mockQueryableToFeedIterator =
            QueryableToFeedIteratorTestDouble.MockFor(mockFeedIterator.Object);

        CosmosDbQueryHandler queryHandler =
            new(mockQueryableToFeedIterator.Object, mockContainerProvider.Object);

        // Act
        IEnumerable<TestResponseObject> result =
            await queryHandler.ReadItemsAsync<TestResponseObject>(
                "containerKey",
                "SELECT * FROM c",
                CancellationToken.None);

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
    public async Task ReadItemsAsync_LinqQuery_ReturnsExpectedItems()
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

        CosmosDbQueryHandler queryHandler =
            new(mockQueryableToFeedIterator.Object, mockContainerProvider.Object);

        // Act
        IEnumerable<TestResponseObject> result =
            await queryHandler.ReadItemsAsync<TestResponseObject>(
                "containerKey",
                item => item,
                item => item.Id == "1",
                CancellationToken.None);

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
    public void Constructor_WithNullContainerProvider_ThrowsExpectedArgumentNullException()
    {
        // Arrange.
        Mock<IQueryableToFeedIterator> mockQueryableToFeedIterator =
            QueryableToFeedIteratorTestDouble.MockFor(
                FeedIteratorTestDouble.DefaultMock<TestResponseObject>().Object);

        // Act
        Action failedAction = () =>
            new CosmosDbQueryHandler(
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
        Mock <Container> mockContainer =
            CosmosContainerTestDouble.MockFor(orderedTestItems);
        Mock<ICosmosDbContainerProvider> mockContainerProvider =
            ContainerProviderTestDouble.MockFor(mockContainer.Object);

        // Act
        Action failedAction = () =>
            new CosmosDbQueryHandler(
                queryableToFeedIterator: null!, mockContainerProvider.Object);

        // Assert
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(failedAction);

        exception.Message.Should().Be("Value cannot be null. (Parameter 'queryableToFeedIterator')");
    }

    [Theory]
    [InlineData(null!, typeof(ArgumentNullException))]
    [InlineData("", typeof(ArgumentException))]
    public Task ReadItemByIdAsync_WithNullOrEmptyId_ThrowsExpectedArgumentExceptionType(
        string id, Type expectedException)
    {
        // Arrange  
        CosmosDbQueryHandler queryHandler = CreateDefaultQueryHandler();

        // Act, assert 
        return Assert.ThrowsAsync(expectedException, () =>
             queryHandler.ReadItemByIdAsync<TestResponseObject>(
                id,
                containerKey: "containerKey",
                partitionKeyValue: "partitionKeyValue"));
    }

    [Theory]
    [InlineData(null!, typeof(ArgumentNullException))]
    [InlineData("", typeof(ArgumentException))]
    public Task ReadItemByIdAsync_WithNullOrEmptyContainerKey_ThrowsExpectedArgumentExceptionType(
        string containerKey, Type expectedException)
    {
        // Arrange  
        CosmosDbQueryHandler queryHandler = CreateDefaultQueryHandler();

        // Act, assert 
        return Assert.ThrowsAsync(expectedException, () =>
             queryHandler.ReadItemByIdAsync<TestResponseObject>(
                id: "test-id",
                containerKey,
                partitionKeyValue: "partitionKeyValue"));
    }

    [Theory]
    [InlineData(null!, typeof(ArgumentNullException))]
    [InlineData("", typeof(ArgumentException))]
    public Task ReadItemByIdAsync_WithNullOrEmptyPartitionKeyValue_ThrowsExpectedArgumentExceptionType(
        string partitionKeyValue, Type expectedException)
    {
        // Arrange  
        CosmosDbQueryHandler queryHandler = CreateDefaultQueryHandler();

        // Act, assert 
        return Assert.ThrowsAsync(expectedException, () =>
             queryHandler.ReadItemByIdAsync<TestResponseObject>(
                id: "id",
                containerKey: "containerKey",
                partitionKeyValue));
    }

    [Theory]
    [InlineData(null!, typeof(ArgumentNullException))]
    [InlineData("", typeof(ArgumentException))]
    public Task ReadItemsAsync_WithNullOrEmptyContainerKey_ThrowsExpectedArgumentExceptionType(
        string containerKey, Type expectedException)
    {
        // Arrange  
        CosmosDbQueryHandler queryHandler = CreateDefaultQueryHandler();
        Expression<Func<TestResponseObject, TestResponseObject>> selector = testItem => testItem;
        Expression<Func<TestResponseObject, bool>> predicate = testItem => testItem.Id == "1";

        // Act, assert 
        return Assert.ThrowsAsync(expectedException, () =>
             queryHandler.ReadItemsAsync<TestResponseObject>(containerKey, selector, predicate));
    }

    [Fact]
    public async Task ReadItemsAsync_WithNullSelector_ThrowsExpectedArgumentNullExceptionType()
    {
        // Arrange  
        CosmosDbQueryHandler queryHandler = CreateDefaultQueryHandler();
        Expression<Func<TestResponseObject, TestResponseObject>> selector = null!;
        Expression<Func<TestResponseObject, bool>> predicate = testItem => testItem.Id == "1";

        // Act, assert 
        var exception = Assert.ThrowsAsync<ArgumentNullException>(() =>
             queryHandler.ReadItemsAsync<TestResponseObject>(containerKey: "containerKey", selector, predicate));

        // Check the exception message
        Assert.Equal("Value cannot be null. (Parameter 'selector')", (await exception).Message);
    }

    [Fact]
    public async Task ReadItemsAsync_WithNullPredictae_ThrowsExpectedArgumentNullExceptionType()
    {
        // Arrange  
        CosmosDbQueryHandler queryHandler = CreateDefaultQueryHandler();
        Expression<Func<TestResponseObject, TestResponseObject>> selector = testItem => testItem;
        Expression<Func<TestResponseObject, bool>> predicate = null!;

        // Act, assert 
        var exception = Assert.ThrowsAsync<ArgumentNullException>(() =>
             queryHandler.ReadItemsAsync<TestResponseObject>(containerKey: "containerKey", selector, predicate));

        // Check the exception message
        Assert.Equal("Value cannot be null. (Parameter 'predicate')", (await exception).Message);
    }

    /// <summary>
    /// Creates a default instance of <see cref="CosmosDbQueryHandler"/> for testing purposes.
    /// </summary>
    private static CosmosDbQueryHandler CreateDefaultQueryHandler()
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

        return new CosmosDbQueryHandler(
            mockQueryableToFeedIterator.Object, mockContainerProvider.Object);
    }
}