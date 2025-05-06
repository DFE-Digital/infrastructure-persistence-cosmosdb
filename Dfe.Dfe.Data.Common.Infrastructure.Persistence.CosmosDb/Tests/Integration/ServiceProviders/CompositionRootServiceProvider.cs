using DfE.Data.ComponentLibrary.Infrastructure.Persistence.CosmosDb;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration.ServiceProviders;

public sealed class CompositionRootServiceProvider
{
    public IServiceProvider SetUpServiceProvider(IConfiguration configuration)
    {
        IServiceCollection services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(configuration);
        services.AddCosmosDbDependencies();
        return services.BuildServiceProvider();
    }
}