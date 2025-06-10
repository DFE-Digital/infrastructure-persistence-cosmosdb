using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Options;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration.Fixture.Configuration;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration.Fixture.Model;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration.Fixture.ServiceProviders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration.Fixture;

/// <summary>
/// Provides helper methods for setting up isolated Cosmos DB test environments.
/// </summary>
public sealed class CosmosDbTestHelper : IDisposable
{
    /// <summary>
    /// A collection of container records used for testing purposes.
    /// </summary>
    public IReadOnlyCollection<ContainerRecord>? ContainerRecords { get; private set; }

    /// <summary>
    /// The Cosmos DB context used for creating and managing the test database and container.
    /// </summary>
    private CosmosDbContext DbContext { get; } = new CosmosDbContext();

    /// <summary>
    /// Creates a fully isolated Cosmos DB context, populates it with test data,
    /// and returns the query handler and test data for use in integration tests.
    /// </summary>
    /// <param name="numberOfRecords">The number of test records to generate and insert.</param>
    /// <returns>
    /// A tuple containing:
    /// - The initialized ICosmosDbQueryHandler
    /// - The CosmosDbContext (for cleanup)
    /// - The collection of inserted test records
    /// </returns>
    public async Task<(THandlerType queryHandler, CosmosDbContext context, IReadOnlyCollection<ContainerRecord> records)>
    CreateIsolatedDbContext<THandlerType>(string containerName, int numberOfRecords = 400) where THandlerType : notnull
    {
        // Build configuration using stubbed emulator settings.
        IConfiguration configurationSettings =
            new ConfigurationSettingsBuilder()
                .SetupConfiguration(CosmosDbConfigurationStub.GetEmulatorConfiguration(containerName));

        // Set up the service provider with all required services and dependencies.
        IServiceProvider testServiceProvider =
            new CompositionRootServiceProvider()
                .ConfigureServiceProvider(configurationSettings);

        // Retrieve repository options (e.g., endpoint, keys, container config).
        IOptions<RepositoryOptions> repositoryOptions =
            testServiceProvider.GetRequiredService<IOptions<RepositoryOptions>>();

        // Generate a collection of fake container records for testing.
        ContainerRecords = InitialiseContainerRecords(numberOfRecords);

        // Create and populate the test database and container with generated records.
        await DbContext.CreateAndPopulate(repositoryOptions.Value, ContainerRecords);

        // Resolve the Cosmos DB handler from the service provider.
        THandlerType cosmosHandler = testServiceProvider.GetRequiredService<THandlerType>();

        // Return the handler, context, and test data for use in integration tests.
        return (cosmosHandler, DbContext, ContainerRecords);
    }

    /// <summary>
    /// Generates a collection of fake container records using the Bogus library.
    /// </summary>
    /// <param name="numberOfRecords">The number of records to generate.</param>
    /// <returns>A read-only collection of generated container records.</returns>
    private IReadOnlyCollection<ContainerRecord> InitialiseContainerRecords(int numberOfRecords) =>
        new Bogus.Faker<ContainerRecord>()
            .StrictMode(true) // Ensures all properties must be explicitly defined.
            .RuleFor(containerRecord => containerRecord.id, _ => Guid.NewGuid().ToString()) // Unique ID.
            .RuleFor(containerRecord => containerRecord.username, fake => fake.Internet.UserName()) // Fake username.
            .RuleFor(containerRecord => containerRecord.pk, (_, containerRecord) => containerRecord.id) // Partition key = ID.
            .Generate(numberOfRecords);

    /// <summary>
    /// Cleans up resources by disposing of the Cosmos DB context.
    /// </summary>
    public void Dispose()
    {
        DbContext.CleanUpResources();
    }
}
