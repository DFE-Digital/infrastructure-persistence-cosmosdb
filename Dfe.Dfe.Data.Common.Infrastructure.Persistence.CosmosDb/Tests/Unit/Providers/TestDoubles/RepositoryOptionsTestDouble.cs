using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Options;
using Microsoft.Extensions.Options;
using Moq;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Unit.Providers.TestDoubles;

/// <summary>
/// Test double for RepositoryOptions to create stub data for testing purposes.
/// </summary>
internal class RepositoryOptionsTestDouble
{
    /// <summary>
    /// Creates a default mock of IOptions<RepositoryOptions>.
    /// </summary>
    /// <returns>A new Mock<IOptions<RepositoryOptions>> instance.</returns>
    public static Mock<IOptions<RepositoryOptions>> DefaultMock() => new Mock<IOptions<RepositoryOptions>>();

    /// <summary>
    /// Sets up the mock to return a RepositoryOptions stub with the specified container name and partition key.
    /// </summary>
    /// <param name="containerName">The name of the container.</param>
    /// <param name="partitionKey">The partition key for the container.</param>
    public static Mock<IOptions<RepositoryOptions>> MockFor(string containerName, string partitionKey)
    {
        // Create a default mock of IOptions<RepositoryOptions>
        Mock<IOptions<RepositoryOptions>> mockRepositoryOptions = DefaultMock();

        // Setup the mock to return a RepositoryOptions stub when the Value property is accessed
        mockRepositoryOptions.Setup(options => options.Value)
            .Returns(CreateRepositoryOptionsStub(containerName, partitionKey)).Verifiable();

        return mockRepositoryOptions;
    }

    /// <summary>
    /// Creates a stub for RepositoryOptions with the specified container name and partition key.
    /// </summary>
    /// <param name="containerName">The name of the container.</param>
    /// <param name="partitionKey">The partition key for the container.</param>
    /// <returns>A RepositoryOptions object with the specified container and partition key.</returns>
    public static RepositoryOptions CreateRepositoryOptionsStub(string containerName, string partitionKey) =>
        new()
        {
            // Initializing the Containers property with a list of dictionaries
            Containers =
                // Adding a dictionary with the container name as the key and ContainerOptions as the value
                new Dictionary<string, ContainerOptions> {
                    { containerName,
                        new ContainerOptions {
                            ContainerName = containerName,
                            PartitionKey = partitionKey
                        }
                    }
                }
        };
}
