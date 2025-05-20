using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Handlers.Query;
using Moq;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Unit.Repositories.TestDoubles;

/// <summary>
/// Provides test doubles for the <see cref="ICosmosDbQueryHandler"/> interface.
/// </summary>
internal static class CosmosDbQueryHandlerTestDouble
{
    /// <summary>
    /// Creates a default mock of the <see cref="ICosmosDbQueryHandler"/> interface.
    /// </summary>
    /// <returns>
    /// A default mock of the <see cref="ICosmosDbQueryHandler"/> interface.
    /// </returns>
    public static Mock<ICosmosDbQueryHandler> DefaultMock() => new();

    /// <summary>
    /// Creates a mock of the <see cref="ICosmosDbQueryHandler"/> interface that returns a specified response.
    /// </summary>
    /// <typeparam name="TResponse">
    /// The type of the response object.
    /// </typeparam>
    /// <param name="response">
    /// The response object to be returned by the mock.
    /// </param>
    /// <returns>
    /// The provisioned response object.
    /// </returns>
    public static Mock<ICosmosDbQueryHandler> MockForSingle<TResponse>(TResponse response) where TResponse : class
    {
        // Get a default mock instance
        Mock<ICosmosDbQueryHandler> mockQueryHandler = DefaultMock();

        // Set up the mock to return the provided response regardless of input parameters
        mockQueryHandler.Setup(queryHandler =>
            queryHandler.ReadItemByIdAsync<TResponse>(
                It.IsAny<string>(),         // id
                It.IsAny<string>(),         // partition key
                It.IsAny<string>(),         // container name
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response)         // return the provided response
            .Verifiable();

        return mockQueryHandler;
    }

    /// <summary>
    /// Creates a mock of the <see cref="ICosmosDbQueryHandler"/> interface that returns a specified response.
    /// </summary>
    /// <typeparam name="TResponse">
    /// The type of the response object.
    /// </typeparam>
    /// <param name="response">
    /// The response object to be returned by the mock.
    /// </param>
    /// <returns>
    /// The provisioned response object.
    /// </returns>
    public static Mock<ICosmosDbQueryHandler> MockForMultiple<TResponse>(IEnumerable<TResponse> response) where TResponse : class
    {
        // Get a default mock instance
        Mock<ICosmosDbQueryHandler> mockQueryHandler = DefaultMock();

        // Set up the mock to return the provided response regardless of input parameters
        mockQueryHandler.Setup(queryHandler =>
            queryHandler.ReadItemsAsync<TResponse>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response)
            .Verifiable();

        return mockQueryHandler;
    }
}
