namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Options;

/// <summary>
/// Represents configuration options for a Cosmos DB container.
/// </summary>
public sealed class ContainerOptions
{
    /// <summary>
    /// Gets or sets the name of the Cosmos DB container.
    /// </summary>
    public string? ContainerName { get; set; }

    /// <summary>
    /// Gets or sets the partition key used for efficient data retrieval and distribution.
    /// </summary>
    public string? PartitionKey { get; set; }
}