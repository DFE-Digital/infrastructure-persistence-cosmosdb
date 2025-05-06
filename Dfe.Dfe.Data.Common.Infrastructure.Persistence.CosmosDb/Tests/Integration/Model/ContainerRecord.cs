namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration.Model;

public sealed class ContainerRecord : IContainerRecord
{
    public string id { get; set; } = string.Empty;

    public string pk { get; set; } = string.Empty;

    public string username { get; set; } = string.Empty;
}
