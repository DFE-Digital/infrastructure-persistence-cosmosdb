using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Handlers.Query;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Providers;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Unit.Handlers.Query.TestDoubles;
using FluentAssertions;
using Microsoft.Azure.Cosmos;
using Moq;
using System.Linq.Expressions;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Unit.Handlers.Query;

public class CosmosDbQueryHandlerTests
{
    [Fact]
    public async Task ReadItemByIdAsync_ReturnsExpectedItem()
    {
        // Arrange
        ItemResponse<TestItem>  mockItemResponse =
            ItemResponseTestDouble.MockFor(new TestItem { Id = "1", Name = "TestItem" });
        Mock<Container> mockContainer =
            CosmosContainerTestDouble.MockFor(mockItemResponse);
        Mock<ICosmosDbContainerProvider> mockContainerProvider =
            ContainerProviderTestDouble.MockFor(mockContainer.Object);
        Mock<IQueryableToFeedIterator> mockQueryableToFeedIterator =
            QueryableToFeedIteratorTestDouble.MockFor(
                FeedIteratorTestDouble.DefaultMock<TestItem>().Object);

        CosmosDbQueryHandler queryHandler =
            new(mockQueryableToFeedIterator.Object, mockContainerProvider.Object);

        // Act
        TestItem result =
            await queryHandler.ReadItemByIdAsync<TestItem>("1", "containerKey", "partitionKey");

        // Assert - we'll just check the values even though they're mocked to ensure
        // the right depth of calls is being made etc although we'll still verify these calls.
        Assert.NotNull(result);
        Assert.Equal("1", result.Id);
        Assert.Equal("TestItem", result.Name);

        // Ensure the correct two calls were made to the mocked container.
        mockContainer.Verify(container =>
            container.ReadItemAsync<TestItem>(
                It.IsAny<string>(), It.IsAny<PartitionKey>(), default, default), Times.Once);

        // Ensure the correct single call was made to the mocked container provider.
        mockContainerProvider.Verify(containerProvider =>
            containerProvider.GetContainerAsync("containerKey"), Times.Once);
    }

    [Fact]
    public async Task ReadItemsAsync_ReturnsExpectedItems()
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
            CosmosContainerTestDouble.MockFor(mockFeedIterator.Object);
        Mock<ICosmosDbContainerProvider> mockContainerProvider =
            ContainerProviderTestDouble.MockFor(mockContainer.Object);
        Mock<IQueryableToFeedIterator> mockQueryableToFeedIterator =
            QueryableToFeedIteratorTestDouble.MockFor(mockFeedIterator.Object);

        CosmosDbQueryHandler queryHandler =
            new(mockQueryableToFeedIterator.Object, mockContainerProvider.Object);

        // Act
        IEnumerable<TestItem> result =
            await queryHandler.ReadItemsAsync<TestItem>(
                "containerKey",
                "SELECT * FROM c",
                CancellationToken.None);

        // Assert - we'll just check the values even though they're mocked to ensure
        // the right depth of calls is being made etc although we'll still verify these calls.
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Collection(result,
            item => Assert.Equal("1", item.Id),
            item => Assert.Equal("2", item.Id));

        // Ensure the correct single call was made to the mocked feed iterator.
        mockFeedIterator.Verify(feedIterator =>
            feedIterator.ReadNextAsync(It.IsAny<CancellationToken>()), Times.Once);

        // Ensure the correct single call was made to the mocked container.
        mockContainer.Verify(container =>
            container.ReadItemAsync<TestItem>(
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
                FeedIteratorTestDouble.DefaultMock<TestItem>().Object);

        // Act
        Action failedAction = () =>
            new CosmosDbQueryHandler(
                mockQueryableToFeedIterator.Object, cosmosDbContainerProvider: null!);

        // Assert
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(failedAction);

        exception.Message.Should().Be("Value cannot be null. (Parameter 'cosmosDbContainerProvider')");
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
             queryHandler.ReadItemByIdAsync<TestItem>(
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
             queryHandler.ReadItemByIdAsync<TestItem>(
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
             queryHandler.ReadItemByIdAsync<TestItem>(
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
        Expression<Func<TestItem, TestItem>> selector = testItem => testItem;
        Expression<Func<TestItem, bool>> predicate = testItem => testItem.Id == "1";

        // Act, assert 
        return Assert.ThrowsAsync(expectedException, () =>
             queryHandler.ReadItemsAsync<TestItem>(containerKey, selector, predicate));
    }

    [Fact]
    public async Task ReadItemsAsync_WithNullSelector_ThrowsExpectedArgumentNullExceptionType()
    {
        // Arrange  
        CosmosDbQueryHandler queryHandler = CreateDefaultQueryHandler();
        Expression<Func<TestItem, TestItem>> selector = null!;
        Expression<Func<TestItem, bool>> predicate = testItem => testItem.Id == "1";

        // Act, assert 
        var exception = Assert.ThrowsAsync<ArgumentNullException>(() =>
             queryHandler.ReadItemsAsync<TestItem>(containerKey: "containerKey", selector, predicate));

        // Check the exception message
        Assert.Equal("Value cannot be null. (Parameter 'selector')", (await exception).Message);
    }

    [Fact]
    public async Task ReadItemsAsync_WithNullPredictae_ThrowsExpectedArgumentNullExceptionType()
    {
        // Arrange  
        CosmosDbQueryHandler queryHandler = CreateDefaultQueryHandler();
        Expression<Func<TestItem, TestItem>> selector = testItem => testItem;
        Expression<Func<TestItem, bool>> predicate = null!;

        // Act, assert 
        var exception = Assert.ThrowsAsync<ArgumentNullException>(() =>
             queryHandler.ReadItemsAsync<TestItem>(containerKey: "containerKey", selector, predicate));

        // Check the exception message
        Assert.Equal("Value cannot be null. (Parameter 'predicate')", (await exception).Message);
    }

    /// <summary>
    /// Test item class used for mocking and testing purposes.
    /// </summary>
    public class TestItem
    {
        /// <summary>
        /// Unique identifier for the test item.
        /// </summary>
        public string? Id { get; set; }
        /// <summary>
        /// Name of the test item.
        /// </summary>
        public string? Name { get; set; }
    }

    /// <summary>
    /// Creates a default instance of <see cref="CosmosDbQueryHandler"/> for testing purposes.
    /// </summary>
    /// <returns></returns>
    private static CosmosDbQueryHandler CreateDefaultQueryHandler()
    {
        ItemResponse<TestItem> mockItemResponse =
            ItemResponseTestDouble.MockFor<TestItem>(null!);
        Mock<Container> mockContainer =
            CosmosContainerTestDouble.MockFor(mockItemResponse);
        Mock<ICosmosDbContainerProvider> mockContainerProvider =
            ContainerProviderTestDouble.MockFor(mockContainer.Object);
        Mock<IQueryableToFeedIterator> mockQueryableToFeedIterator =
            QueryableToFeedIteratorTestDouble.MockFor(
                FeedIteratorTestDouble.DefaultMock<TestItem>().Object);

        return new CosmosDbQueryHandler(
            mockQueryableToFeedIterator.Object, mockContainerProvider.Object);
    }
}