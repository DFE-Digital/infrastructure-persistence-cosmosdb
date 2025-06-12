using Microsoft.Azure.Cosmos;
using Moq;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Unit.Handlers.TestDoubles;

/// <summary>
/// Provides utility methods for mocking Cosmos DB containers in unit tests.
/// </summary>
internal static class CosmosContainerTestDouble
{
    /// <summary>
    /// Creates a default mock instance of a Cosmos DB container.
    /// </summary>
    /// <returns>A mocked <see cref="Container"/> instance.</returns>
    public static Mock<Container> DefaultMock() => new();

    /// <summary>
    /// Mocks the <see cref="ReadItemAsync{T}"/> method to return a specified response.
    /// </summary>
    /// <typeparam name="TResponse">The expected response type.</typeparam>
    /// <param name="response">The mocked response object.</param>
    /// <returns>A <see cref="Mock{T}"/> of <see cref="Container"/> with predefined read behavior.</returns>
    public static Mock<Container> MockFor<TResponse>(ItemResponse<TResponse> response)
    {
        Mock<Container> containerMock = DefaultMock();

        // Configure the mock to return a predefined response when ReadItemAsync is called.
        containerMock.Setup(container => container.ReadItemAsync<TResponse>(
            It.IsAny<string>(),
            It.IsAny<PartitionKey>(),
            default, default))
            .ReturnsAsync(response)
            .Verifiable();

        return containerMock;
    }

    /// <summary>
    /// Mocks the <see cref="GetItemQueryIterator{T}"/> method to return a predefined query response.
    /// </summary>
    /// <typeparam name="TResponse">The expected response type.</typeparam>
    /// <param name="response">The mocked <see cref="FeedIterator{T}"/> object.</param>
    /// <returns>A mock of <see cref="Container"/> with query iterator setup.</returns>
    public static Mock<Container> MockFor<TResponse>(FeedIterator<TResponse> response)
    {
        Mock<Container> containerMock = DefaultMock();

        // Setup mock to return predefined query iterator results.
        containerMock.Setup(container =>
            container.GetItemQueryIterator<TResponse>(
                It.IsAny<QueryDefinition>(),
                It.IsAny<string>(),
                It.IsAny<QueryRequestOptions>()))
            .Returns(response)
            .Verifiable();

        // Setup the mock to return a mocked Database instance when accessed.
        containerMock.SetupGet(container => container.Database)
            .Returns(new Mock<Database>().Object)
            .Verifiable();

        return containerMock;
    }

    /// <summary>
    /// Mocks the <see cref="GetItemLinqQueryable{T}"/> method for LINQ queries.
    /// </summary>
    /// <typeparam name="TResponse">The expected response type.</typeparam>
    /// <param name="response">The <see cref="IOrderedQueryable{T}"/> response.</param>
    /// <returns>A mock of <see cref="Container"/> that supports LINQ queries.</returns>
    public static Mock<Container> MockFor<TResponse>(IOrderedQueryable<TResponse> response)
    {
        Mock<Container> containerMock = DefaultMock();

        // Configure the mock to return a predefined response for LINQ queries.
        containerMock.Setup(container => container.GetItemLinqQueryable<TResponse>(
            It.IsAny<bool>(),
            It.IsAny<string>(),
            It.IsAny<QueryRequestOptions>(),
            It.IsAny<CosmosLinqSerializerOptions>()))
            .Returns(response);

        // Setup a mocked Database instance for consistency.
        containerMock.SetupGet(container => container.Database)
            .Returns(new Mock<Database>().Object)
            .Verifiable();

        return containerMock;
    }

    /// <summary>
    /// Mocks the <see cref="CreateItemAsync{T}"/> method for item creation.
    /// </summary>
    /// <typeparam name="TResponse">The expected response type.</typeparam>
    /// <param name="response">The mocked item response.</param>
    /// <returns>A mock of <see cref="Container"/> with predefined item creation behavior.</returns>
    public static Mock<Container> MockCreateItemFor<TResponse>(ItemResponse<TResponse> response)
    {
        Mock<Container> containerMock = DefaultMock();

        // Setup mock to return predefined response for CreateItemAsync.
        containerMock.Setup(container =>
            container.CreateItemAsync(response.Resource, It.IsAny<PartitionKey>(), null, default))
            .ReturnsAsync(response)
            .Verifiable();

        return containerMock;
    }

    /// <summary>
    /// Mocks the <see cref="UpsertItemAsync{T}"/> method for item upsertion.
    /// </summary>
    /// <typeparam name="TResponse">The expected response type.</typeparam>
    /// <param name="response">The mocked upsert item response.</param>
    /// <returns>A mock of <see cref="Container"/> supporting upsert operations.</returns>
    public static Mock<Container> MockUpsertItemFor<TResponse>(ItemResponse<TResponse> response)
    {
        Mock<Container> containerMock = DefaultMock();

        // Configure mock to return predefined response for UpsertItemAsync.
        containerMock.Setup(container =>
            container.UpsertItemAsync(response.Resource, It.IsAny<PartitionKey>(), null, default))
            .ReturnsAsync(response)
            .Verifiable();

        return containerMock;
    }

    /// <summary>
    /// Mocks the <see cref="ReplaceItemAsync{T}"/> method for item replacements.
    /// </summary>
    /// <typeparam name="TResponse">The expected response type.</typeparam>
    /// <param name="response">The mocked replace item response.</param>
    /// <returns>A mock of <see cref="Container"/> supporting replace operations.</returns>
    public static Mock<Container> MockReplaceItemFor<TResponse>(ItemResponse<TResponse> response)
    {
        Mock<Container> containerMock = DefaultMock();

        // Configure mock to return predefined response for UpsertItemAsync.
        containerMock.Setup(container =>
            container.ReplaceItemAsync(response.Resource, It.IsAny<string>(), It.IsAny<PartitionKey>(), null, default))
            .ReturnsAsync(response)
            .Verifiable();

        return containerMock;
    }

    /// <summary>
    /// Mocks the <see cref="DeleteItemAsync{T}"/> method for item deletion.
    /// </summary>
    /// <typeparam name="TResponse">The expected response type.</typeparam>
    /// <param name="response">The mocked delete item response.</param>
    /// <returns>A mock of <see cref="Container"/> supporting item deletion.</returns>
    public static Mock<Container> MockDeleteItemFor<TResponse>(ItemResponse<TResponse> response)
    {
        Mock<Container> containerMock = DefaultMock();

        // Setup mock to return predefined response for DeleteItemAsync.
        containerMock.Setup(container =>
            container.DeleteItemAsync<TResponse>(It.IsAny<string>(), It.IsAny<PartitionKey>(), null, default))
            .ReturnsAsync(response)
            .Verifiable();

        return containerMock;
    }
}
