using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Handlers.Command;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Handlers.Query;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Options;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DfE.Data.ComponentLibrary.Infrastructure.Persistence.CosmosDb;

/// <summary>
/// The composition root provides a unified location where the composition
/// of the object graphs for the application take place, using the IOC container.
/// </summary>
public static class CompositionRoot
{
    /// <summary>
    /// Configures Cosmos DB dependencies for the hosting application.
    /// </summary>
    /// <param name="services">The service collection to which dependencies are added.</param>
    /// <returns>The configured service collection.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="services"/> parameter is null.</exception>
    public static IServiceCollection AddCosmosDbDependencies(this IServiceCollection services)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services),
                "A service collection is required to configure the CosmosDb Repository.");
        }

        // Configure repository options using the application configuration.
        services.AddOptions<RepositoryOptions>()
            .Configure<IConfiguration>(
                (settings, configuration) =>
                    configuration.GetSection(nameof(RepositoryOptions)).Bind(settings));

        // Register Cosmos DB providers and repositories as singleton services.
        services.TryAddSingleton<ICosmosDbClientProvider, CosmosDbClientProvider>();
        services.TryAddSingleton<ICosmosDbContainerProvider, CosmosDbContainerProvider>();
        services.TryAddSingleton<ICosmosDbQueryHandler, CosmosDbQueryHandler>();
        services.TryAddSingleton<IQueryableToFeedIterator, QueryableToFeedIterator>();
        services.TryAddSingleton<ICosmosDbCommandHandler, CosmosDbCommandHandler>();

        return services;
    }
}