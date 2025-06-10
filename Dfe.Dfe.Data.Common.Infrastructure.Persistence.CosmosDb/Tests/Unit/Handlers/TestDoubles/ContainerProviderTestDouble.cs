using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Providers;
using Microsoft.Azure.Cosmos;
using Moq;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Unit.Handlers.TestDoubles;

/// <summary>
/// Test double for the <see cref="ICosmosDbContainerProvider"/> interface.
/// </summary>
internal static class ContainerProviderTestDouble
{
    /// <summary>
    /// Creates a default mock of the <see cref="ICosmosDbContainerProvider"/> interface.
    /// </summary>
    /// <returns>
    /// A <see cref="Mock{ICosmosDbContainerProvider}"/> instance configured with default behavior.
    /// </returns>
    public static Mock<ICosmosDbContainerProvider> DefaultMock() => new();

    /// <summary>
    /// Creates a mock of the <see cref="ICosmosDbContainerProvider"/> interface
    /// that returns a specified <see cref="Container"/> instance.
    /// </summary>
    /// <param name="response">
    /// The <see cref="Container"/> instance to be returned when the mock is called.
    /// </param>
    /// <returns>
    /// A <see cref="Mock{ICosmosDbContainerProvider}"/> instance configured with default behavior.
    /// </returns>
    public static Mock<ICosmosDbContainerProvider> MockFor(Container response)
    {
        Mock<ICosmosDbContainerProvider> containerProviderMock = DefaultMock();

        containerProviderMock
             .Setup(containerProvider => containerProvider.GetContainerAsync("containerKey"))
             .ReturnsAsync(response).Verifiable();

        return containerProviderMock;
    }
}