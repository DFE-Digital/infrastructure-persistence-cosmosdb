using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Handlers.Query;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Repositories;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Unit.Repositories.TestDoubles;
using Moq;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Unit.Repositories;

public class ReadOnlyRepositoryTests
{
    [Fact]
    public async Task GetItemByIdAsync_ShouldReturnItem()
    {
        // Arrange
        var testItem = new TestItem { Id = "1", Name = "TestItem" };
        Mock<ICosmosDbQueryHandler> mockQueryHandler =
            CosmosDbQueryHandlerTestDouble.MockForSingle(testItem);

        // Act
        await new ReadOnlyRepository(mockQueryHandler.Object)
            .GetItemByIdAsync<TestItem>("1", "containerKey", "partitionKey");

        // Verify
        mockQueryHandler.Verify(queryHandler =>
                queryHandler.ReadItemByIdAsync<TestItem>(
                    It.IsAny<string>(),         // id
                    It.IsAny<string>(),         // partition key
                    It.IsAny<string>(),         // container name
                    It.IsAny<CancellationToken>()),
                Times.Once);
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

        Mock<ICosmosDbQueryHandler> mockQueryHandler =
            CosmosDbQueryHandlerTestDouble.MockForMultiple(testItems);

        // Act
        IEnumerable<TestItem> result =
            await new ReadOnlyRepository(mockQueryHandler.Object)
                .GetAllItemsByQueryAsync<TestItem>("SELECT * FROM c", "containerKey");

        // Verify
        mockQueryHandler.Verify(mockQueryHandler =>
                mockQueryHandler.ReadItemsAsync<TestItem>(
                    It.IsAny<string>(),         // query
                    It.IsAny<string>(),         // container name
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }

    /// <summary>
    /// Represents a simple data model used for testing purposes.
    /// Contains basic properties such as an identifier and a name.
    /// </summary>
    public class TestItem
    {
        /// <summary>
        /// Gets or sets the unique identifier for the test item.
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// Gets or sets the name associated with the test item.
        /// </summary>
        public string? Name { get; set; }
    }

}