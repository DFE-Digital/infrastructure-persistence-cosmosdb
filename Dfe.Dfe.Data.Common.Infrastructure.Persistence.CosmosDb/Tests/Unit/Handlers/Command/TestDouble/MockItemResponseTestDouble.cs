using Microsoft.Azure.Cosmos;
using Moq;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Unit.Handlers.Command.TestDouble;

/// <summary>
/// Provides utility methods for mocking Cosmos DB responses.
/// </summary>
internal static class MockItemResponseTestDouble
{
    /// <summary>
    /// Creates a default mock instance of <see cref="ItemResponse{TResponse}"/>.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response resource.</typeparam>
    /// <returns>A mock of <see cref="ItemResponse{TResponse}"/>.</returns>
    public static Mock<ItemResponse<TResponse>> DefaultMock<TResponse>() where TResponse : class => new();

    /// <summary>
    /// Creates a mock instance of <see cref="ItemResponse{TResponse}"/> with a predefined resource value.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response resource.</typeparam>
    /// <param name="response">The response object to return when accessed.</param>
    /// <returns>A mock of <see cref="ItemResponse{TResponse}"/> with the specified resource.</returns>
    public static Mock<ItemResponse<TResponse>> MockItemResponseFor<TResponse>(TResponse response) where TResponse : class
    {
        Mock<ItemResponse<TResponse>> mock = DefaultMock<TResponse>();

        // Set up the mock to return the provided response object when accessing the Resource property
        mock.Setup(itemResponse => itemResponse.Resource).Returns(response);

        return mock;
    }
}
