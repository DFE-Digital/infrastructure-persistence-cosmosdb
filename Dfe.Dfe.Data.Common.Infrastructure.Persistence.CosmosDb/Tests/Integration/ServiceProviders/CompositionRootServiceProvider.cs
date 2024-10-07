using DfE.Data.ComponentLibrary.Infrastructure.Persistence.CosmosDb;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration.ServiceProviders;

[SuppressMessage("Microsoft.Performance", "CD1600: The class must have a documentation header.")]
[SuppressMessage("Microsoft.Performance", "ClassDocumentationHeader: The class must have a documentation header.")]
public sealed class CompositionRootServiceProvider
{
    [SuppressMessage("Microsoft.Performance", "CD1605: The method must have a documentation header.")]
    [SuppressMessage("Microsoft.Performance", "MethodDocumentationHeader: The method must have a documentation header.")]
    public IServiceProvider SetUpServiceProvider(IConfiguration configuration)
    {
        IServiceCollection services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(configuration);
        services.AddCosmosDbDependencies();
        return services.BuildServiceProvider();
    }
}