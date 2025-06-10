using Microsoft.Azure.Cosmos;
using Moq;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Unit.Handlers.TestDoubles;

/// <summary>
/// Provides a mocked instance of a Cosmos DB container for testing purposes.
/// </summary>
internal static class CosmosContainerTestDouble
{
    /// <summary>
    /// Creates a default mock of the Cosmos DB container.
    /// </summary>
    /// <returns>
    /// A mocked instance of a cosmos db container.
    /// </returns>
    public static Mock<Container> DefaultMock() => new();

    /// <summary>
    /// Mocks the ReadItemAsync method of the Cosmos DB container to return a specified response.
    /// </summary>
    /// <typeparam name="TResponse">
    /// The type of the response expected from the ReadItemAsync method.
    /// </typeparam>
    /// <param name="response">
    /// The response returned by the mocked read method call.
    /// </param>
    /// <returns>
    /// A mocked instance of the Cosmos DB container with the specified response for ReadItemAsync.
    /// </returns>
    public static Mock<Container> MockFor<TResponse>(ItemResponse<TResponse> response)
    {
        Mock<Container> containerMock = DefaultMock();

        containerMock
            .Setup(container => container.ReadItemAsync<TResponse>(
                It.IsAny<string>(),
                It.IsAny<PartitionKey>(), default, default))
            .ReturnsAsync(response).Verifiable();

        return containerMock;
    }

    /// <summary>
    /// Mocks the GetItemQueryIterator method of the Cosmos DB container to return a specified response.
    /// </summary>
    /// <typeparam name="TResponse">
    /// The type of the response expected from the GetItemQueryIterator method.
    /// </typeparam>
    /// <param name="response">
    /// The response returned by the mocked GetItemQueryIterator method call.
    /// </param>
    /// <returns>
    /// A mocked instance of the Cosmos DB container with the specified response for GetItemQueryIterator.
    /// </returns>
    public static Mock<Container> MockFor<TResponse>(FeedIterator<TResponse> response)
    {
        Mock<Container> containerMock = DefaultMock();

        containerMock
            .Setup(container =>
                container.GetItemQueryIterator<TResponse>(
                    It.IsAny<QueryDefinition>(),
                    It.IsAny<string>(),
                    It.IsAny<QueryRequestOptions>()))
            .Returns(response).Verifiable();

        containerMock
            .SetupGet(container =>
                container.Database).Returns(new Mock<Database>().Object);

        return containerMock;
    }

    /// <summary>
    /// Sets up a mock for <see cref="Container"/> to return a specified IOrderedQueryable<TResponse>.
    /// </summary>
    /// <typeparam name="TResponse">
    /// The type of the response items.
    /// </typeparam>
    /// <param name="response">
    /// The IOrderedQueryable<TResponse> to be returned by the mock.
    /// </param>
    /// <returns>
    /// A mock object of type <see cref="Container"/> set up to return the specified IOrderedQueryable<TResponse>.
    /// </returns>
    public static Mock<Container> MockFor<TResponse>(IOrderedQueryable<TResponse> response)
    {
        // Create a default mock object for Container.
        Mock<Container> containerMock = DefaultMock();

        // Setup the mock to return the provided response when GetItemLinqQueryable is called.
        containerMock.Setup(container =>
            container.GetItemLinqQueryable<TResponse>(
                It.IsAny<bool>(),
                It.IsAny<string>(),
                It.IsAny<QueryRequestOptions>(),
                It.IsAny<CosmosLinqSerializerOptions>()))
            .Returns(response);

        // Setup the mock to return a new mock Database object when the Database property is accessed.
        containerMock
            .SetupGet(container =>
                container.Database).Returns(new Mock<Database>().Object);

        return containerMock;
    }

    public static Mock<Container> MockXXXXFor<TResponse>(TResponse response)
    {
        // Create a default mock object for Container.
        Mock<Container> containerMock = DefaultMock();

        //containerMock.Setup(c => c.CreateItemAsync(response, It.IsAny<PartitionKey>(), null, default))
        //.ReturnsAsync(response).Verifiable();

        return containerMock;
    }
}