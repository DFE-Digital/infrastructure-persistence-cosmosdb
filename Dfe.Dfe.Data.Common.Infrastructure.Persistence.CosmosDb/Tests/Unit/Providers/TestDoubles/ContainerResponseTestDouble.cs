using Microsoft.Azure.Cosmos;
using Moq;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Unit.Providers.TestDoubles
{
    /// <summary>
    /// Provides test doubles for ContainerResponse.
    /// </summary>
    internal static class ContainerResponseTestDouble
    {
        /// <summary>
        /// Creates a default mock of <see cref="ContainerResponse"/>.
        /// </summary>
        /// <returns>A new Mock<ContainerResponse> instance.</returns>
        public static Mock<ContainerResponse> DefaultMock() => new();

        /// <summary>
        /// Creates a mock of <see cref="ContainerResponse"/>.
        /// </summary>
        /// <returns>A configured Mock<DatabaseResponse> instance.</returns>
        public static Mock<ContainerResponse> MockFor(Container container)
        {
            // Create a default mock of ContainerResponse
            Mock<ContainerResponse> mockContainerResponse = DefaultMock();

            // Setup the mock to return a default mock of Container when the container response is accessed.
            mockContainerResponse.Setup(response =>
                response.Container).Returns(container).Verifiable();

            return mockContainerResponse;
        }
    }
}
