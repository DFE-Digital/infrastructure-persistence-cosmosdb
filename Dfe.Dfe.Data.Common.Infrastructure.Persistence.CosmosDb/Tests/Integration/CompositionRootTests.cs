using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Handlers.Command;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Handlers.Query;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Options;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Providers;
using DfE.Data.ComponentLibrary.Infrastructure.Persistence.CosmosDb;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration;

public class CompositionRootTests
{
    [Fact]
    public void AddCosmosDbDependencies_NullServiceCollection_ThrowsArgumentNullException()
    {
        // arrange
        IServiceCollection services = null!;

        // act
        var exception =
            Assert.Throws<ArgumentNullException>(() =>
                services.AddCosmosDbDependencies());

        // assert
        Assert.Equal("services", exception.ParamName);
    }

    [Fact]
    public void AddCosmosDbDependencies_RegistersExpectedServices()
    {
        // arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();
        services.AddSingleton<IConfiguration>(configuration);

        // act
        services.AddCosmosDbDependencies();
        var provider = services.BuildServiceProvider();

        // assert
        Assert.NotNull(provider.GetService<ICosmosDbClientProvider>());
        Assert.NotNull(provider.GetService<ICosmosDbContainerProvider>());
        Assert.NotNull(provider.GetService<ICosmosDbQueryHandler>());
        Assert.NotNull(provider.GetService<IQueryableToFeedIterator>());
        Assert.NotNull(provider.GetService<ICosmosDbCommandHandler>());
        Assert.NotNull(provider.GetService<IOptions<RepositoryOptions>>());
    }
}