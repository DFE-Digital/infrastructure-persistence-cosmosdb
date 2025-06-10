using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Handlers.Command;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Providers;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Unit.Handlers.TestDoubles;
using Microsoft.Azure.Cosmos;
using Moq;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Unit.Handlers.Command;

public class CosmosDbCommandHandlerTests
{
    private readonly Mock<ICosmosDbContainerProvider> _containerProviderMock;
    private readonly Mock<Container> _containerMock;
    private readonly CosmosDbCommandHandler _handler;

    public CosmosDbCommandHandlerTests()
    {
        _containerMock = new Mock<Container>();
        _containerProviderMock = ContainerProviderTestDouble.MockFor(_containerMock.Object);
        _handler = new CosmosDbCommandHandler(_containerProviderMock.Object);
    }

    [Fact]
    public void Constructor_NullProvider_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new CosmosDbCommandHandler(null!));
    }

    [Fact]
    public async Task CreateItemAsync_WithPartitionKeyValue_CreatesItem()
    {
        TestItem item = new() { Id = "1" };
        Mock<ItemResponse<TestItem>> responseMock = MockItemResponse(item);

        _containerMock.Setup(c => c.CreateItemAsync(item, It.IsAny<PartitionKey>(), null, default))
            .ReturnsAsync(responseMock.Object);

        _containerProviderMock.Setup(p => p.GetContainerAsync("test")).ReturnsAsync(_containerMock.Object);

        var result = await _handler.CreateItemAsync(item, "test", "1");

        Assert.Equal(item, result);
    }

    [Fact]
    public async Task UpdateItemAsync_WithPartitionKey_UpdatesItem()
    {
        TestItem item = new() { Id = "1" };

        Mock<ItemResponse<TestItem>> responseMock = MockItemResponse(item);

        _containerMock.Setup(container =>
            container.UpsertItemAsync(item, It.IsAny<PartitionKey>(), null, default))
                .ReturnsAsync(responseMock.Object);
        
        _containerProviderMock.Setup(containerProvider =>
            containerProvider.GetContainerAsync("test"))
                .ReturnsAsync(_containerMock.Object);

        TestItem result = await _handler.UpdateItemAsync(item, "test", new PartitionKey("1"));

        Assert.Equal(item, result);
    }

    [Fact]
    public Task DeleteItemAsync_WithPartitionKey_DeletesItem()
    {
        _containerMock.Setup(c => c.DeleteItemAsync<TestItem>("1", It.IsAny<PartitionKey>(), null, default))
            .ReturnsAsync(Mock.Of<ItemResponse<TestItem>>());
        _containerProviderMock.Setup(p => p.GetContainerAsync("test")).ReturnsAsync(_containerMock.Object);

        return _handler.DeleteItemAsync<TestItem>("1", "test", new PartitionKey("1"));
    }

    [Theory]
    [InlineData(null!, typeof(ArgumentNullException))]
    [InlineData("", typeof(ArgumentException))]
    public Task CreateItemAsync_NullPartitionKey_Throws(string? partitionKey, Type expectedException)
    {
        // Act, assert 
        return Assert.ThrowsAsync(expectedException, () =>
             _handler.CreateItemAsync(new TestItem(), "test", partitionKey!));
    }

    private Mock<ItemResponse<T>> MockItemResponse<T>(T item) where T : class
    {
        var mock = new Mock<ItemResponse<T>>();
        mock.Setup(r => r.Resource).Returns(item);
        return mock;
    }

    public class TestItem
    {
        public string Id { get; set; } = string.Empty;
    }
}