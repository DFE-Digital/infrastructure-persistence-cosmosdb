using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Handlers.Command;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Providers;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Unit.Handlers.Command.TestDouble;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Unit.Handlers.TestDoubles;
using Microsoft.Azure.Cosmos;
using Moq;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Unit.Handlers.Command;

public class CosmosDbCommandHandlerTests
{
    [Fact]
    public void Constructor_NullProvider_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new CosmosDbCommandHandler(null!));
    }

    [Fact]
    public async Task CreateItemAsync_WithPartitionKeyValue_CreatesItem()
    {
        // arrange
        TestItem item = new() { Id = "1" };
        Mock<ItemResponse<TestItem>> responseMock =
            MockItemResponseTestDouble.MockItemResponseFor(item);
        Mock<Container> containerMock =
            CosmosContainerTestDouble.MockCreateItemFor(responseMock.Object);
        Mock<ICosmosDbContainerProvider> containerProviderMock =
            ContainerProviderTestDouble.MockFor(containerMock.Object);

        // act
        TestItem? result =
            await new CosmosDbCommandHandler(
                containerProviderMock.Object).CreateItemAsync(item, "containerKey", "1");

        // assert/verify
        Assert.Equal(item, result);

        containerMock.Verify(container =>
            container.CreateItemAsync<TestItem>(
                It.IsAny<TestItem>(),
                It.Is<PartitionKey>(pk => pk == new PartitionKey("1")),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateItemAsync_WithPartitionKey_UpdatesItem()
    {
        // arrange
        TestItem item = new() { Id = "1" };
        Mock<ItemResponse<TestItem>> responseMock =
            MockItemResponseTestDouble.MockItemResponseFor(item);
        Mock<Container> containerMock =
            CosmosContainerTestDouble.MockUpsertItemFor(responseMock.Object);
        Mock<ICosmosDbContainerProvider> containerProviderMock =
            ContainerProviderTestDouble.MockFor(containerMock.Object);

        // act
        TestItem? result =
            await new CosmosDbCommandHandler(containerProviderMock.Object)
                .UpdateItemAsync(item, "containerKey", new PartitionKey("1"));

        // assert/verify
        Assert.Equal(item, result);

        containerMock.Verify(container =>
            container.UpsertItemAsync<TestItem>(
                It.IsAny<TestItem>(),
                It.Is<PartitionKey>(pk => pk == new PartitionKey("1")),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteItemAsync_WithPartitionKey_DeletesItem()
    {
        // arrange
        Mock<Container> containerMock =
            CosmosContainerTestDouble.MockDeleteItemFor(Mock.Of<ItemResponse<TestItem>>());
        Mock<ICosmosDbContainerProvider> containerProviderMock =
            ContainerProviderTestDouble.MockFor(containerMock.Object);

        // act
        await new CosmosDbCommandHandler(containerProviderMock.Object)
            .DeleteItemAsync<TestItem>("1", "containerKey", new PartitionKey("1"));

        // verify
        containerMock.Verify(container =>
            container.DeleteItemAsync<TestItem>(
                It.IsAny<string>(),
                It.Is<PartitionKey>(pk => pk == new PartitionKey("1")),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(null!, typeof(ArgumentNullException))]
    [InlineData("", typeof(ArgumentException))]
    public Task CreateItemAsync_NullPartitionKey_Throws(string? partitionKey, Type expectedException)
    {
        // arrange
        Mock<Container> containerMock =
            CosmosContainerTestDouble.MockDeleteItemFor(Mock.Of<ItemResponse<TestItem>>());
        Mock<ICosmosDbContainerProvider> containerProviderMock =
            ContainerProviderTestDouble.MockFor(containerMock.Object);

        // Act, assert 
        return Assert.ThrowsAsync(expectedException, () =>
              new CosmosDbCommandHandler(
                containerProviderMock.Object).CreateItemAsync(new TestItem(), "test", partitionKey!));
    }

    public class TestItem
    {
        public string Id { get; set; } = string.Empty;
    }
}