using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Options;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Providers;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Unit.Providers.TestDoubles;
using FluentAssertions;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Linq.Expressions;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Unit.Providers;

public class CosmosDbContainerProviderTests
{
    [Fact]
    public async Task GetContainerAsync_ShouldReturnContainer()
    {
        // Arrange
        const string ContainerName = "TestContainer";
        const string PartitionKey = "/id";

        Mock<ILogger<CosmosDbContainerProvider>> mockLoggerTestDouble = LoggerTestDouble.DefaultMock();
        Mock <IOptions<RepositoryOptions>> mockRepositoryOptions =
            RepositoryOptionsTestDouble.MockFor(ContainerName, PartitionKey);
        Mock<ContainerResponse> mockContainerResponse =
            ContainerResponseTestDouble.MockFor(new Mock<Container>().Object);
        Mock<Database> mockDatabase =
            CosmosDatabaseTestDouble.MockFor(mockContainerResponse.Object);
        Mock<DatabaseResponse> mockDatabaseResponse =
            CosmosDatabaseResponseTestDouble.MockFor(mockDatabase.Object);
        Mock<ICosmosDbClientProvider> mockCosmosDbClientProvider =
            CosmosDbClientProviderTestDouble.MockFor(mockDatabaseResponse.Object);

        CosmosDbContainerProvider containerProvider =
            new(mockLoggerTestDouble.Object, mockCosmosDbClientProvider.Object, mockRepositoryOptions.Object);

        // Act
        var result = await containerProvider.GetContainerAsync(containerKey: ContainerName);

        // Assert - we'll just check the values even though they're mocked to ensure
        // the mocks are operating correctly although we'll still verify these calls.
        Assert.NotNull(result);

        // Ensure the correct call was made to the mocked container instance.
        mockContainerResponse.Verify(response => response.Container, Times.Once());
        // Ensure the correct call was made to the mocked database response instance.
        mockDatabaseResponse.Verify(response => response.Database, Times.Once());
        // Ensure the correct call was made to the mocked database instance.
        mockDatabase.Verify(database =>
             database.CreateContainerIfNotExistsAsync(
                It.IsAny<string>(), It.IsAny<string>(), default, default, default), Times.Once);
        // Ensure the correct call was made to the client provider instance.
        mockCosmosDbClientProvider.Verify(clientProvider =>
            clientProvider.InvokeCosmosClientAsync(
                It.IsAny<Func<CosmosClient, Task<DatabaseResponse>>>()), Times.Once());
        // Ensure the correct call was made to the repository options instance.
        mockRepositoryOptions.Verify(options => options.Value, Times.Once());
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsExpectedArgumentNullException()
    {
        // Arrange.
        Mock<ICosmosDbClientProvider> mockCosmosDbClientProvider =
            CosmosDbClientProviderTestDouble.DefaultMock();
        Mock<IOptions<RepositoryOptions>> mockRepositoryOptions =
            RepositoryOptionsTestDouble.DefaultMock();

        // Act
        Action failedAction = () =>
            new CosmosDbContainerProvider(
                logger: null!, mockCosmosDbClientProvider.Object, mockRepositoryOptions.Object);

        // Assert
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(failedAction);

        exception.Message.Should().Be("Value cannot be null. (Parameter 'logger')");
    }

    [Fact]
    public void Constructor_WithNullCosmosDbClientProvider_ThrowsExpectedArgumentNullException()
    {
        // Arrange.
        Mock<IOptions<RepositoryOptions>> mockRepositoryOptions =
            RepositoryOptionsTestDouble.DefaultMock();
        Mock<ILogger<CosmosDbContainerProvider>> mockLoggerTestDouble = LoggerTestDouble.DefaultMock();

        // Act
        Action failedAction = () =>
            new CosmosDbContainerProvider(
                mockLoggerTestDouble.Object, cosmosClientProvider: null!, mockRepositoryOptions.Object);

        // Assert
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(failedAction);

        exception.Message.Should().Be("Value cannot be null. (Parameter 'cosmosClientProvider')");
    }

    [Fact]
    public async Task ReadItemsAsync_WithNullSelector_ThrowsExpectedArgumentNullExceptionType()
    {
        // Arrange
        Mock<IOptions<RepositoryOptions>> mockRepositoryOptions =
            RepositoryOptionsTestDouble.DefaultMock();
        Mock<ICosmosDbClientProvider> mockCosmosDbClientProvider =
            CosmosDbClientProviderTestDouble.DefaultMock();
        Mock<ILogger<CosmosDbContainerProvider>> mockLoggerTestDouble = LoggerTestDouble.DefaultMock();

        CosmosDbContainerProvider containerProvider =
            new(mockLoggerTestDouble.Object, mockCosmosDbClientProvider.Object, mockRepositoryOptions.Object);

        // Act, assert 
        var exception = Assert.ThrowsAsync<ArgumentNullException>(() =>
             containerProvider.GetContainerAsync(containerKey: null!));

        // Check the exception message
        Assert.Equal("Value cannot be null. (Parameter 'containerKey')", (await exception).Message);
    }

    [Fact]
    public async Task GetContainerAsync_ShouldLogErrorOnCosmosException()
    {
        // Arrange
        const string ContainerName = "TestContainer";
        const string PartitionKey = "/id";

        Mock<ICosmosDbClientProvider> mockCosmosDbClientProvider =
            CosmosDbClientProviderTestDouble.MockForCosmosError();
        Mock<IOptions<RepositoryOptions>> mockRepositoryOptions =
            RepositoryOptionsTestDouble.MockFor(ContainerName, PartitionKey);

        Expression<Action<ILogger<CosmosDbContainerProvider>>> logAction =
            logger =>
                logger.Log<It.IsAnyType>(LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>());

        Mock<ILogger<CosmosDbContainerProvider>> mockLogger = LoggerTestDouble.MockFor(logAction);

        CosmosDbContainerProvider containerProvider =
            new(mockLogger.Object, mockCosmosDbClientProvider.Object, mockRepositoryOptions.Object);

        // Act & Assert
        await Assert.ThrowsAsync<CosmosException>(() =>
            containerProvider.GetContainerAsync(containerKey: ContainerName));

        // Verify the log action was called with the expected parameters.
        mockLogger.Verify(logAction, Times.Once);
    }

    [Fact]
    public async Task GetContainerAsync_ShouldLogErrorOnGenericException()
    {
        // Arrange
        const string ContainerName = "TestContainer";
        const string PartitionKey = "/id";

        Mock<ICosmosDbClientProvider> mockCosmosDbClientProvider =
            CosmosDbClientProviderTestDouble.MockForGenericError();
        Mock<IOptions<RepositoryOptions>> mockRepositoryOptions =
            RepositoryOptionsTestDouble.MockFor(ContainerName, PartitionKey);

        Expression<Action<ILogger<CosmosDbContainerProvider>>> logAction =
            logger =>
                logger.Log<It.IsAnyType>(LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>());

        Mock<ILogger<CosmosDbContainerProvider>> mockLogger = LoggerTestDouble.MockFor(logAction);

        CosmosDbContainerProvider containerProvider =
            new(mockLogger.Object, mockCosmosDbClientProvider.Object, mockRepositoryOptions.Object);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() =>
            containerProvider.GetContainerAsync(containerKey: ContainerName));

        // Verify the log action was called with the expected parameters.
        mockLogger.Verify(logAction, Times.Once);
    }
}