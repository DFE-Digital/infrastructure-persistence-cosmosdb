using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Options;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Options.Extensions;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Unit.Options.Extensions;

public class RepositoryOptionsExtensionsTests
{
    [Fact]
    public void GetContainerOptions_ShouldReturnContainerOptions_WhenContainerExists()
    {
        // Arrange
        const string containerKey = "TestContainer";
        var expectedContainerOptions =
            new ContainerOptions { ContainerName = containerKey, PartitionKey = "/id" };

        var repositoryOptions = new RepositoryOptions
        {
            Containers = [
                new Dictionary<string, ContainerOptions>{
                    { containerKey, expectedContainerOptions }
                }
            ]
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
            Containers = [
                new Dictionary<string, ContainerOptions> {
                    { "ExistingContainer",
                        new ContainerOptions {
                            ContainerName = "ExistingContainer", PartitionKey = "/id"
                        }
                    }
                }
            ]
        };

        const string missingContainerKey = "MissingContainer";

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            repositoryOptions.GetContainerOptions(missingContainerKey));

        Assert.Contains($"Container with key: {missingContainerKey} not configured in options.", exception.Message);
    }

    [Fact]
    public void TryGetContainerOptionsDictionary_ShouldReturnContainerDictionary_WhenContainerExists()
    {
        // Arrange
        const string containerKey = "TestContainer";
        var expectedContainerOptions = new Dictionary<string, ContainerOptions>
        {
            { containerKey, new ContainerOptions { ContainerName = "TestContainer", PartitionKey = "/id" } }
        };

        var repositoryOptions = new RepositoryOptions
        {
            Containers = [expectedContainerOptions]
        };

        // Act
        var result = repositoryOptions.TryGetContainerOptionsDictionary(containerKey);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.ContainsKey(containerKey));
    }

    [Fact]
    public void TryGetContainerOptionsDictionary_ShouldThrowInvalidOperationException_WhenContainerDoesNotExist()
    {
        // Arrange
        var repositoryOptions = new RepositoryOptions
        {
            Containers = []
        };

        const string missingContainerKey = "MissingContainer";

        // Act & Assert
        var exception =
            Assert.Throws<InvalidOperationException>(() =>
                repositoryOptions.TryGetContainerOptionsDictionary(missingContainerKey));

        Assert.Contains($"Container with key: {missingContainerKey} not configured in options.", exception.Message);
    }
}