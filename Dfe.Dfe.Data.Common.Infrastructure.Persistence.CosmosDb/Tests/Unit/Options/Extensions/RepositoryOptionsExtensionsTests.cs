using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Options;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Options.Extensions;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Unit.Options.Extensions;

public class RepositoryOptionsExtensionsTests
{
    [Fact]
    public void GetContainerOptions_Should_ThrowNull_When_ContainersIsNull()
    {
        // Arrange
        var repositoryOptions = new RepositoryOptions
        {
            Containers = null
        };

        const string containerKey = "TestContainer";

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            repositoryOptions.GetContainerOptions(containerKey));
    }

    [Fact]
    public void GetContainerOptions_ShouldReturnContainerOptions_WhenContainerExists()
    {
        // Arrange
        const string containerKey = "TestContainer";
        var expectedContainerOptions =
            new ContainerOptions { ContainerName = containerKey, PartitionKey = "/id" };

        var repositoryOptions = new RepositoryOptions
        {
            Containers = new Dictionary<string, ContainerOptions>{
                    { containerKey, expectedContainerOptions }
                }

        };

        // Act
        ContainerOptions? result = repositoryOptions.GetContainerOptions(containerKey);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedContainerOptions.ContainerName, result.ContainerName);
        Assert.Equal(expectedContainerOptions.PartitionKey, result.PartitionKey);
    }

    [Fact]
    public void GetContainerOptions_ShouldThrowInvalidOperationException_WhenContainerDoesNotExist()
    {
        // Arrange
        var repositoryOptions = new RepositoryOptions
        {
            Containers = 
                new Dictionary<string, ContainerOptions> {
                    { "ExistingContainer",
                        new ContainerOptions {
                            ContainerName = "ExistingContainer", PartitionKey = "/id"
                        }
                    }
                }
        };

        const string missingContainerKey = "MissingContainer";

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            repositoryOptions.GetContainerOptions(missingContainerKey));

        Assert.Contains($"Container dictionary options with container key: {missingContainerKey} not configured in options.", exception.Message);
    }
}