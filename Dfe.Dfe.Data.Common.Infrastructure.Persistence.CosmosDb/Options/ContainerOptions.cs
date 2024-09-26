namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Options;

/// <summary>
/// 
/// </summary>
public sealed class ContainerOptions
{
    /// <summary>
    /// 
    /// </summary>
    public string? ContainerName { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? PartitionKey { get; set; }
}
