using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Providers;
using DfE.Data.ComponentLibrary.Infrastructure.Persistence.CosmosDb;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration.ServiceProviders;

/// <summary>
/// 
/// </summary>
public sealed class CompositionRootServiceProvider
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public IServiceProvider SetUpServiceProvider(IConfiguration configuration)
    {
        IServiceCollection services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(configuration);
        services.AddCosmosDbDependencies();
        
        return services.BuildServiceProvider();
    }
}
