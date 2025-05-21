using DfE.Data.ComponentLibrary.Infrastructure.Persistence.CosmosDb;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration.Fixture.ServiceProviders;

/// <summary>
/// Responsible for setting up the dependency injection container (service provider)
/// used in integration tests involving Cosmos DB.
/// </summary>
public sealed class CompositionRootServiceProvider
{
    /// <summary>
    /// Configures and builds a service provider with required dependencies.
    /// </summary>
    /// <param name="configuration">The application configuration to be injected into services.</param>
    /// <returns>An IServiceProvider instance with all necessary services registered.</returns>
    public IServiceProvider SetUpServiceProvider(IConfiguration configuration)
    {
        // Create a new service collection to register dependencies
        IServiceCollection services = new ServiceCollection();
        // Add logging services to the container
        services.AddLogging();
        // Register the provided configuration as a singleton service
        services.AddSingleton(configuration);
        // Register Cosmos DB-related dependencies (defined in external library)
        services.AddCosmosDbDependencies();
        // Build and return the service provider
        return services.BuildServiceProvider();
    }
}
