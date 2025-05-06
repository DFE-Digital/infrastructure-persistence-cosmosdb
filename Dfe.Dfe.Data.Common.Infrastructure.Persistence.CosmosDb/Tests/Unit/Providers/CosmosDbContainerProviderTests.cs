using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Options;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Providers;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Unit.Providers;

public class CosmosDbContainerProviderTests
{
    private readonly Mock<ILogger<CosmosDbContainerProvider>> _mockLogger;
    private readonly Mock<ICosmosDbClientProvider> _mockCosmosClientProvider;
    private readonly Mock<IOptions<RepositoryOptions>> _mockRepositoryOptions;
    private readonly CosmosDbContainerProvider _containerProvider;
    private readonly RepositoryOptions _repositoryOptions;

    public CosmosDbContainerProviderTests()
    {
        _mockLogger = new Mock<ILogger<CosmosDbContainerProvider>>();
        _mockCosmosClientProvider = new Mock<ICosmosDbClientProvider>();
        _mockRepositoryOptions = new Mock<IOptions<RepositoryOptions>>();

        _repositoryOptions = new RepositoryOptions
        {
            DatabaseId = "TestDatabase"
        };

        _mockRepositoryOptions.Setup(o => o.Value).Returns(_repositoryOptions);

        _containerProvider = new CosmosDbContainerProvider(
            _mockLogger.Object,
            _mockCosmosClientProvider.Object,
            _mockRepositoryOptions.Object);
    }

    [Fact]
    public async Task GetContainerAsync_ShouldReturnContainer()
    {
        // Arrange
        var mockDatabase = new Mock<Database>();
        var mockContainer = new Mock<Container>();
        var mockContainerOptions = new ContainerOptions { ContainerName = "TestContainer", PartitionKey = "/id" };

        //_mockRepositoryOptions.Setup(o => o.Value.GetContainerOptions(It.IsAny<string>()))
        mockDatabase.Setup(db => db.CreateContainerIfNotExistsAsync(It.IsAny<string>(), It.IsAny<string>(), default, default, default))
            .ReturnsAsync((ContainerResponse)Activator.CreateInstance(typeof(ContainerResponse), true));
        //    .Returns(mockContainerOptions);

        _mockCosmosClientProvider.Setup(p => p.InvokeCosmosClientAsync(It.IsAny<Func<CosmosClient, Task<Database>>>()))
            .ReturnsAsync(mockDatabase.Object);

        // Act
        var result = await _containerProvider.GetContainerAsync("TestContainer");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(mockContainer.Object, result);
    }

    [Fact]
    public async Task GetContainerAsync_ShouldLogErrorOnCosmosException()
    {
        // Arrange
        _mockCosmosClientProvider.Setup(p => p.InvokeCosmosClientAsync(It.IsAny<Func<CosmosClient, Task<Database>>>()))
            .ThrowsAsync(new CosmosException("Cosmos error", System.Net.HttpStatusCode.BadRequest, 0, "", 0));

        // Act & Assert
        await Assert.ThrowsAsync<CosmosException>(() => _containerProvider.GetContainerAsync("TestContainer"));

        _mockLogger.Verify(
            x => x.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.IsAny<EventId>(),
                It.IsAny<object>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<object, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task GetContainerAsync_ShouldLogErrorOnGenericException()
    {
        // Arrange
        _mockCosmosClientProvider.Setup(p => p.InvokeCosmosClientAsync(It.IsAny<Func<CosmosClient, Task<Database>>>()))
            .ThrowsAsync(new Exception("Generic error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _containerProvider.GetContainerAsync("TestContainer"));

        _mockLogger.Verify(
            x => x.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.IsAny<EventId>(),
                It.IsAny<object>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<object, Exception, string>>()),
            Times.Once);
    }
}