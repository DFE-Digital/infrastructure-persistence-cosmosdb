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
    /// 
    /// </summary>
    [JsonProperty("Containers")]
    public List<Dictionary<string, ContainerOptions>>? Containers { get; set; }
}
