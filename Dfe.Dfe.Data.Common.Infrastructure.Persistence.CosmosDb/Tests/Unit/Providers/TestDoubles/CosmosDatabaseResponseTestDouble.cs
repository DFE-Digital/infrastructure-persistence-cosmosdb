using Microsoft.Azure.Cosmos;
using Moq;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Unit.Providers.TestDoubles;

/// <summary>
/// Provides test doubles for DatabaseResponse.
/// </summary>
internal static class CosmosDatabaseResponseTestDouble
{
    /// <summary>
    /// Creates a default mock of <see cref="DatabaseResponse"/>.
    /// </summary>
    /// <returns>A new Mock<DatabaseResponse> instance.</returns>
    public static Mock<DatabaseResponse> DefaultMock() => new();

    /// <summary>
    /// Creates a mock of <see cref="DatabaseResponse"/> and sets up the Database property.
    /// </summary>
    /// <returns>A configured Mock<DatabaseResponse> instance.</returns>
    public static Mock<DatabaseResponse> MockFor(Database database)
    {
        // Create a default mock of DatabaseResponse
        Mock<DatabaseResponse> mockDatabaseResponse = DefaultMock();

        // Setup the mock to return a default mock of Database when the Database property is accessed.
        mockDatabaseResponse.Setup(response =>
            response.Database).Returns(database).Verifiable();

        return mockDatabaseResponse;
    }
}
