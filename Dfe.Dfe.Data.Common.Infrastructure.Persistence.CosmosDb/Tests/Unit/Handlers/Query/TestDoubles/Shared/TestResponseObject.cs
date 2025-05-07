namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Unit.Handlers.Query.TestDoubles.Shared;

/// <summary>
/// An object we can use to mock test responses.
/// </summary>
public sealed class TestResponseObject
{
    /// <summary>
    /// Unique identifier for the test item.
    /// </summary>
    public string? Id { get; set; }
    /// <summary>
    /// Name of the test item.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Factory method to allow new instances of the test response object to be created.
    /// </summary>
    /// <param name="id">
    /// Unique identifier for the test item.
    /// </param>
    /// <param name="name">
    /// Name of the test item.
    /// </param>
    /// <returns>
    /// An initialised test response object.
    /// </returns>
    public static TestResponseObject Create(string id, string name)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(nameof(Id));
        ArgumentNullException.ThrowIfNullOrWhiteSpace(nameof(Name));

        return new TestResponseObject { Id = id, Name = name };
    }
}
