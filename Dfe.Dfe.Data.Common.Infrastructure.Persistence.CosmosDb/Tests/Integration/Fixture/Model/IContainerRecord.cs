namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration.Fixture.Model;

/// <summary>
/// Defines the contract for a container record used in Cosmos DB integration tests.
/// Any class implementing this interface must have an 'id' property.
/// </summary>
public interface IContainerRecord
{
    /// <summary>
    /// Gets or sets the unique identifier for the record.
    /// This typically corresponds to the 'id' field in a Cosmos DB document.
    /// </summary>
    string id { get; set; }
}
