namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration.Fixture;

/// <summary>
/// Provides stubbed configuration settings for the Cosmos DB emulator,
/// used in integration testing scenarios.
/// </summary>
public static class CosmosDbConfigurationStub
{
    /// <summary>
    /// The partition key path used in the container.
    /// </summary>
    private const string PartitionKey = "/pk";

    /// <summary>
    /// The ID of the database used in the Cosmos DB emulator.
    /// </summary>
    private const string DatabaseId = "integration-test";

    /// <summary>
    /// The local endpoint URI for the Cosmos DB emulator.
    /// </summary>
    private const string EndpointUri = "https://localhost:8081";

    /// <summary>
    /// The primary key for the Cosmos DB emulator. This is safe to hard-code for local testing purposes.
    /// </summary>
    private const string PrimaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

    /// <summary>
    /// Returns a dictionary of configuration settings for the Cosmos DB emulator.
    /// </summary>
    public static Dictionary<string, string?> GetEmulatorConfiguration(string containerName) =>
        new()
        {
            { "RepositoryOptions:EndpointUri", EndpointUri },
            { "RepositoryOptions:PrimaryKey", PrimaryKey },
            { "RepositoryOptions:DatabaseId", DatabaseId },
            { "RepositoryOptions:ConnectionMode", "1" },
            { $"RepositoryOptions:Containers:{containerName}:ContainerName", containerName },
            { $"RepositoryOptions:Containers:{containerName}:PartitionKey", PartitionKey }
        };
}
