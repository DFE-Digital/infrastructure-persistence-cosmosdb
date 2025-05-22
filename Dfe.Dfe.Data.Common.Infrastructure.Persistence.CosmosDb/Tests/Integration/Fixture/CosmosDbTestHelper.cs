using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Handlers.Query;
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
public static class CosmosDbTestHelper
{
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
    public static async Task<
        (ICosmosDbQueryHandler queryHandler, CosmosDbContext context, IReadOnlyCollection<ContainerRecord> records)>
        CreateIsolatedQueryHandlerAsync(int numberOfRecords = 1000)
    {
        // Build configuration using the Cosmos DB emulator settings
        IConfiguration configurationSettings =
            new ConfigurationSettingsBuilder().SetupConfiguration(CosmosDbConfigurationStub.GetEmulatorConfiguration());

        // Set up the service provider with all required dependencies
        IServiceProvider testServiceProvider =
            new CompositionRootServiceProvider().SetUpServiceProvider(configurationSettings);

        // Retrieve repository options (e.g., endpoint, keys, container config)
        var repositoryOptions = testServiceProvider.GetRequiredService<IOptions<RepositoryOptions>>();

        // Create a new Cosmos DB context for this test run
        var cosmosDbContext = new CosmosDbContext();

        // Generate a collection of test records
        var containerRecords = new CosmosDbContextFixture().InitialiseContainerRecords(numberOfRecords);

        // Create the database and container, and populate it with test data
        await cosmosDbContext.CreateAndPopulate(repositoryOptions.Value, containerRecords);

        // Resolve the query handler from the service provider
        var queryHandler = testServiceProvider.GetRequiredService<ICosmosDbQueryHandler>();

        // Return all components needed for the test
        return (queryHandler, cosmosDbContext, containerRecords);
    }
}
