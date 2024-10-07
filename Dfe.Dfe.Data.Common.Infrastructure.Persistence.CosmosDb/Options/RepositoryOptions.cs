using Newtonsoft.Json;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Options;

/// <summary>
/// 
/// </summary>
public sealed class RepositoryOptions
{
    /// <summary>
    /// 
    /// </summary>
    public string? EndpointUri { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public string? PrimaryKey { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? DatabaseId { get; set; }

    /// <summary>
    /// Represents the connection mode to be used by the client when connecting to the Azure Cosmos DB service.
    /// Gateway = 0: uses the Azure Cosmos DB gateway to route all requests to the Azure Cosmos DB service. The gateway proxies
    /// requests to the right data partition. Use Gateway connectivity when within firewall settings do not allow Direct connectivity.
    /// All connections are made to the database account's endpoint through the standard HTTPS port (443).
    /// Direct = 1: uses direct connectivity to connect to the data nodes in the Azure Cosmos DB service. Use gateway only to initialize and
    /// cache logical addresses and refresh on updates. Use Direct connectivity for best performance. Connections are made to the
    /// data on a range of port numbers either using HTTPS or TCP/SSL.
    /// </summary>
    public int ConnectionMode { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonProperty("Containers")]
    public List<Dictionary<string, ContainerOptions>>? Containers { get; set; }
}
