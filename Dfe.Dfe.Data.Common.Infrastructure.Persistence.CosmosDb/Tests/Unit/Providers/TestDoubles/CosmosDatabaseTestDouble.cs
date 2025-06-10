using Microsoft.Azure.Cosmos;
using Moq;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Unit.Providers.TestDoubles;

/// <summary>
/// Provides test doubles for Cosmos.Database.
/// </summary>
internal static class CosmosDatabaseTestDouble
{
    /// <summary>
    /// Creates a default mock of <see cref="Database"/>.
    /// </summary>
    /// <returns>A new Mock<Database> instance.</returns>
    public static Mock<Database> DefaultMock() => new();

    /// <summary>
    /// Creates a mock of <see cref="Database"/> and sets up the Database property.
    /// </summary>
    /// <returns>A configured Mock<Database> instance.</returns>
    public static Mock<Database> MockFor(ContainerResponse containerResponse)
    {
        // Create a default mock of Database.
        Mock<Database> mockDatabase = DefaultMock();

        // Setup the mock to return a default mock of ContainerResponse when the Database property is accessed.
        mockDatabase.Setup(database =>
            database.CreateContainerIfNotExistsAsync(
                It.IsAny<string>(), It.IsAny<string>(), default, default, default))
            .Returns(Task.FromResult(containerResponse)).Verifiable();

        return mockDatabase;
    }
}
