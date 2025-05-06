using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Handlers.Query;
using Microsoft.Azure.Cosmos;
using Moq;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Unit.Handlers.Query.TestDoubles;

/// <summary>
/// Provides test doubles for Cosmos LINQ queries using Moq.
/// Contains methods to create default and customized mock
/// objects for the <see cref="IQueryableToFeedIterator"/>.
/// </summary>
internal static class QueryableToFeedIteratorTestDouble
{
    /// <summary>
    /// Creates a default mock object of type <see cref="IQueryableToFeedIterator"/>.
    /// </summary>
    /// <returns>
    /// A new mock object of type <see cref="IQueryableToFeedIterator"/>.
    /// </returns>
    public static Mock<IQueryableToFeedIterator> DefaultMock() => new();

    /// <summary>
    /// Sets up a mock for <see cref="IQueryableToFeedIterator"/>
    /// to return a specified <see cref="FeedIterator{TItem}"/>.
    /// </summary>
    /// <typeparam name="TItem">
    /// The type of items returned by the <see cref="FeedIterator{TItem}"/>.
    /// </typeparam>
    /// <param name="feedIterator">
    /// The <see cref="FeedIterator{TItem}"/> to be returned by the mock.
    /// </param>
    /// <returns>
    /// A mock object of type <see cref="IQueryableToFeedIterator"/>
    /// set up to return the specified <see cref="FeedIterator{TItem}"/>.
    /// </returns>
    public static Mock<IQueryableToFeedIterator> MockFor<TItem>(FeedIterator<TItem> feedIterator)
    {
        // Create a default mock object
        Mock<IQueryableToFeedIterator> feedIteratorMock = DefaultMock();

        // Setup the mock to return the provided feedIterator
        // when GetFeedIterator is called with any IQueryable<TItem>.
        feedIteratorMock
             .Setup(feedIterator =>
                feedIterator.GetFeedIterator(
                    It.IsAny<IQueryable<TItem>>()))
             .Returns(feedIterator).Verifiable();

        return feedIteratorMock;
    }
}
