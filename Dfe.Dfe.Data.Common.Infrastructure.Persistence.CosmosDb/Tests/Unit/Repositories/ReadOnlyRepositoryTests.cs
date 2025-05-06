using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Handlers.Query;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Repositories;
using Moq;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Unit.Repositories;

public class ReadOnlyRepositoryTests
{
    private readonly Mock<ICosmosDbQueryHandler> _mockQueryHandler;
    private readonly ReadOnlyRepository _readOnlyRepository;

    public ReadOnlyRepositoryTests()
    {
        _mockQueryHandler = new Mock<ICosmosDbQueryHandler>();
        _readOnlyRepository = new ReadOnlyRepository(_mockQueryHandler.Object);
    }

    [Fact]
    public async Task GetItemByIdAsync_ShouldReturnItem()
    {
        // Arrange
        var testItem = new TestItem { Id = "1", Name = "TestItem" };

        _mockQueryHandler.Setup(queryHandler =>
            queryHandler.ReadItemByIdAsync<TestItem>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(testItem);

        // Act
        var result =
            await _readOnlyRepository
                .GetItemByIdAsync<TestItem>("1", "containerKey", "partitionKey");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("TestItem", result.Name);
    }

    [Fact]
    public async Task GetItemByIdAsync_ShouldThrowException_WhenQueryHandlerIsNull()
    {
        // Act & Assert
        var exception =
            Assert.Throws<ArgumentNullException>(() =>
                new ReadOnlyRepository(null!));

        Assert.Contains("cosmosDbQueryHandler", exception.ParamName);
    }

    [Fact]
    public async Task GetAllItemsByQueryAsync_ShouldReturnItems()
    {
        // Arrange
        var testItems = new List<TestItem>
        {
            new() { Id = "1", Name = "Item1" },
            new() { Id = "2", Name = "Item2" }
        };

        _mockQueryHandler.Setup(queryHandler =>
            queryHandler.ReadItemsAsync<TestItem>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(testItems);

        // Act
        var result =
            await _readOnlyRepository
                .GetAllItemsByQueryAsync<TestItem>("SELECT * FROM c", "containerKey");

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(2, result.Count());
    }

    public class TestItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}