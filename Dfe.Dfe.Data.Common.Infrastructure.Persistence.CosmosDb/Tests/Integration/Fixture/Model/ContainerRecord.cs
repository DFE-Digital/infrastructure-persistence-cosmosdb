namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration.Fixture.Model;

/// <summary>
/// Represents a record stored in a Cosmos DB container for integration testing purposes.
/// Implements the IContainerRecord interface.
/// </summary>
public sealed class ContainerRecord : IContainerRecord
{
    /// <summary>
    /// The unique identifier for the record.
    /// This typically maps to the 'id' field in a Cosmos DB document.
    /// </summary>
    public string id { get; set; } = string.Empty;

    /// <summary>
    /// The partition key used by Cosmos DB to distribute data across partitions.
    /// </summary>
    public string pk { get; set; } = string.Empty;

    /// <summary>
    /// A sample property representing a username associated with the record.
    /// </summary>
    public string username { get; set; } = string.Empty;
}