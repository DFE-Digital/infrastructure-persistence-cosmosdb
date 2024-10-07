using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Handlers.Query;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Options;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Providers;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Repositories;
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
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static IServiceCollection AddCosmosDbDependencies(this IServiceCollection services)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services),
                "A service collection is required to configure the CosmosDb Repository.");
        }

        services.AddOptions<RepositoryOptions>()
            .Configure<IConfiguration>(
                (settings, configuration) =>
                    configuration.GetSection(nameof(RepositoryOptions)).Bind(settings));

        services.TryAddSingleton<ICosmosDbClientProvider, CosmosDbClientProvider>();
        services.TryAddSingleton<ICosmosDbContainerProvider, CosmosDbContainerProvider>();
        services.TryAddSingleton<IReadOnlyRepository, ReadOnlyRepository>();
        services.TryAddSingleton<ICosmosDbQueryHandler, CosmosDbQueryHandler>();

        return services;
    }
}