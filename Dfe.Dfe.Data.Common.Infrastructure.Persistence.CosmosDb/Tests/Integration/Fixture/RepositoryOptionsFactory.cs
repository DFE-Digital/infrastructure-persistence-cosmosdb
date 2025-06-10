using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Options;

namespace DfE.GIAP.Core.IntegrationTests.Fixture;

/// <summary>
/// Factory class for creating <see cref="RepositoryOptions"/> instances used in integration tests.
/// </summary>
internal static class RepositoryOptionsFactory
{
    /// <summary>
    /// Creates a <see cref="RepositoryOptions"/> instance configured for the local Cosmos DB emulator.
    /// </summary>
    /// <returns>A configured <see cref="RepositoryOptions"/> object for local testing.</returns>
    internal static RepositoryOptions LocalCosmosDbEmulator() => new()
    {
        ConnectionMode = 0, // Gateway mode
        EndpointUri = "https://localhost:8081/",
        DatabaseId = "integration-test",
        PrimaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw=="
    };
}