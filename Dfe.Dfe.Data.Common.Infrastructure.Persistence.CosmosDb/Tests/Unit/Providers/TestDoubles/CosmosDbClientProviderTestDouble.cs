using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Providers;
using Microsoft.Azure.Cosmos;
using Moq;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Unit.Providers.TestDoubles
{
    /// <summary>
    /// Provides test doubles for ICosmosDbClientProvider.
    /// </summary>
    internal static class CosmosDbClientProviderTestDouble
    {
        /// <summary>
        /// Creates a default mock of <see cref="ICosmosDbClientProvider"/>.
        /// </summary>
        /// <returns>A new Mock<ICosmosDbClientProvider> instance.</returns>
        public static Mock<ICosmosDbClientProvider> DefaultMock() => new();

        /// <summary>
        /// Creates a mock of <see cref="ICosmosDbClientProvider"/>.
        /// </summary>
        /// <returns>A configured Mock<ICosmosDbClientProvider> instance.</returns>
        public static Mock<ICosmosDbClientProvider> MockFor(DatabaseResponse databaseResponse)
        {
            // Create a default mock of Database.
            Mock<ICosmosDbClientProvider> mockCosmosDbClientProvider = DefaultMock();

            // Setup the mock to return a default mock of DatabaseResponse when the invoke client method is called.
            mockCosmosDbClientProvider.Setup(clientProvider =>
                clientProvider.InvokeCosmosClientAsync(
                    It.IsAny<Func<CosmosClient, Task<DatabaseResponse>>>()))
                .Returns(Task.FromResult(databaseResponse)).Verifiable();

            return mockCosmosDbClientProvider;
        }

        /// <summary>
        /// Creates a mock of <see cref="ICosmosDbClientProvider"/> that simulates a Cosmos DB error.
        /// </summary>
        /// <returns>A new Mock<ICosmosDbClientProvider> instance.</returns>
        public static Mock<ICosmosDbClientProvider> MockForCosmosError()
        {
            // Create a default mock of Database.
            Mock<ICosmosDbClientProvider> mockCosmosDbClientProvider = DefaultMock();

            // Setup the mock to throw a cosmos exception when the invoke client method is called.
            mockCosmosDbClientProvider.Setup(clientProvider =>
                clientProvider.InvokeCosmosClientAsync(
                    It.IsAny<Func<CosmosClient, Task<DatabaseResponse>>>()))
                .ThrowsAsync(new CosmosException("Cosmos error", System.Net.HttpStatusCode.BadRequest, 0, "", 0));

            return mockCosmosDbClientProvider;
        }

        /// <summary>
        /// Creates a mock of <see cref="ICosmosDbClientProvider"/> that simulates a generic error.
        /// </summary>
        /// <returns>A new Mock<ICosmosDbClientProvider> instance.</returns>
        public static Mock<ICosmosDbClientProvider> MockForGenericError()
        {
            // Create a default mock of Database.
            Mock<ICosmosDbClientProvider> mockCosmosDbClientProvider = DefaultMock();

            // Setup the mock to throw a cosmos exception when the invoke client method is called.
            mockCosmosDbClientProvider.Setup(clientProvider =>
                clientProvider.InvokeCosmosClientAsync(
                    It.IsAny<Func<CosmosClient, Task<DatabaseResponse>>>()))
                .ThrowsAsync(new Exception("Generic error"));

            return mockCosmosDbClientProvider;
        }
    }
}
