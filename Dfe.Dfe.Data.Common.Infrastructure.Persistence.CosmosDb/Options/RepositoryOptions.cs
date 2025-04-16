using Newtonsoft.Json;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Options;

/// <summary>
/// Represents configuration options for connecting to an Azure Cosmos DB instance.
/// </summary>
public sealed class RepositoryOptions
{
    /// <summary>
    /// Gets or sets the URI endpoint of the Cosmos DB instance.
    /// </summary>
    public string? EndpointUri { get; set; }

    /// <summary>
    /// Gets or sets the primary key used for authentication with Cosmos DB.
    /// </summary>
    public string? PrimaryKey { get; set; }

    /// <summary>
    /// Gets or sets the ID of the Cosmos DB database.
    /// </summary>
    public string? DatabaseId { get; set; }

    /// <summary>
    /// Gets or sets the collection of container configuration options.
    /// </summary>
    [JsonProperty(nameof(Containers))]
    public List<Dictionary<string, ContainerOptions>>? Containers { get; set; }
}