using Microsoft.Azure.Cosmos;
using Moq;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Unit.Handlers.Query.TestDoubles;

/// <summary>
/// Provides a method to create a default mock of ItemResponse<TResponse>.
/// </summary>
internal static class ItemResponseTestDouble
{
    /// <summary>
    /// Creates a default mock of ItemResponse<TResponse> with default values.
    /// </summary>
    /// <typeparam name="TResponse">
    /// The type of the response object.
    /// </typeparam>
    /// <returns>
    /// A mock of ItemResponse<TResponse> with default values.
    /// </returns>
    public static Mock<ItemResponse<TResponse>> DefaultMock<TResponse>() => new();

    /// <summary>
    /// Creates a mock of ItemResponse<TResponse> with the specified response object.
    /// </summary>
    /// <typeparam name="TResponse">
    /// The type of the response object.
    /// </typeparam>
    /// <param name="response">
    /// The response object to be returned by the mock.
    /// </param>
    /// <returns>
    /// A mock of ItemResponse<TResponse> that returns the specified response object.
    /// </returns>
    public static ItemResponse<TResponse> MockFor<TResponse>(TResponse response)
    {
        Mock<ItemResponse<TResponse>> mockResponse = DefaultMock<TResponse>();
        mockResponse.Setup(response =>
            response.Resource).Returns(response);

        return mockResponse.Object;
    }
}
