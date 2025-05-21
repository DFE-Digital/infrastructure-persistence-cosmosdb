using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration.Fixture.Model;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration.Fixture;

/// <summary>
/// A test fixture that provides a reusable CosmosDbContext instance for integration tests.
/// It also handles cleanup of resources and generates test data.
/// </summary>
public class CosmosDbContextFixture : IDisposable
{
    // Flag to ensure the context is only disposed once
    private bool _contextDisposed;

    /// <summary>
    /// Initializes a new instance of the fixture and creates a CosmosDbContext.
    /// </summary>
    public CosmosDbContextFixture()
    {
        Context = new CosmosDbContext();
    }

    /// <summary>
    /// The Cosmos DB context used for integration testing.
    /// </summary>
    public CosmosDbContext Context { get; }

    /// <summary>
    /// Disposes the fixture and cleans up Cosmos DB resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this); // Prevent finaliser from running.
    }

    /// <summary>
    /// Protected dispose pattern implementation to allow sub-classing.
    /// </summary>
    /// <param name="disposing">True if called from Dispose(), false if from finaliser.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_contextDisposed)
        {
            if (disposing)
            {
                // Clean up Cosmos DB resources
                Context.CleanUpResources();
            }

            _contextDisposed = true;
        }
    }

    /// <summary>
    /// Generates a collection of fake container records for testing.
    /// </summary>
    /// <param name="numberOfRecords">The number of records to generate.</param>
    /// <returns>A read-only collection of generated ContainerRecord objects.</returns>
    public IReadOnlyCollection<ContainerRecord> InitialiseContainerRecords(int numberOfRecords) =>
        new Bogus.Faker<ContainerRecord>()
            .StrictMode(true)                                                                           // Ensures all rules must be defined
            .RuleFor(containerRecord => containerRecord.id, _ => Guid.NewGuid().ToString())             // Unique ID
            .RuleFor(containerRecord => containerRecord.username, fake => fake.Internet.UserName())     // Fake user name.
            .RuleFor(containerRecord => containerRecord.pk, (_, containerRecord) => containerRecord.id) // Partition key = ID
            .Generate(numberOfRecords);                                                                 // Generate the specified number of records
}
