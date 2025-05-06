using Microsoft.Azure.Cosmos;
using Moq;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Unit.Handlers.Query.TestDoubles;

/// <summary>
/// Provides a mock implementation of the FeedIterator class for testing purposes.
/// </summary>
internal static class FeedIteratorTestDouble
{
    /// <summary>
    /// Creates a mock of the FeedIterator class for testing purposes.
    /// </summary>
    /// <typeparam name="TResponse">
    /// The type of the response items in the feed iterator.
    /// </typeparam>
    /// <returns>
    /// A mock of the FeedIterator class that can be used in tests.
    /// </returns>
    public static Mock<FeedIterator<TResponse>> DefaultMock<TResponse>() => new();

    /// <summary>
    /// Creates a mock of the FeedIterator class for testing purposes.
    /// </summary>
    /// <typeparam name="TResponse">
    /// The type of the response items in the feed iterator.
    /// </typeparam>
    /// <param name="response">
    /// The ordered queryable response to be returned by the mock.
    /// </param>
    /// <returns>
    /// A mock of the FeedIterator class that can be used in tests.
    /// </returns>
    public static Mock<FeedIterator<TResponse>> MockFor<TResponse>(IOrderedQueryable<TResponse> response)
    {
        Mock<FeedIterator<TResponse>> mockFeedIterator = DefaultMock<TResponse>();
        mockFeedIterator.SetupSequence(feedIterator => feedIterator.HasMoreResults)
            .Returns(true)      // First call: Has more results
            .Returns(false);    // Second call: No more results

        mockFeedIterator.Setup(feedIterator =>
            feedIterator.ReadNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(MockFeedResponse(response.ToList())).Verifiable();

        return mockFeedIterator;
    }

    /// <summary>
    /// Creates a mock of the FeedResponse class for testing purposes.
    /// </summary>
    /// <typeparam name="TResponse">
    /// The type of the response items in the feed response.
    /// </typeparam>
    /// <param name="items">
    /// The items to be returned in the mock feed response.
    /// </param>
    /// <returns>
    /// A mock of the FeedResponse class that returns the specified items.
    /// </returns>
    private static FeedResponse<TResponse> MockFeedResponse<TResponse>(IEnumerable<TResponse> items)
    {
        Mock<FeedResponse<TResponse>> mockResponse = new();
        mockResponse.Setup(response =>
            response.Resource).Returns(items.ToList()).Verifiable();

        return mockResponse.Object;
    }
}
