using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Options;
using DfE.Data.ComponentLibrary.Infrastructure.Persistence.CosmosDb;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Unit;

public sealed class CompositionRootTests
{

    [Fact]
    public void AddCosmosDbDependencies_Registers_RepositoryOptions_OnlyOnce_When_CalledMultipleTimes()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddCosmosDbDependencies();
        services.AddCosmosDbDependencies();

        // Assert
        int repositoryOptionsRegistrationCount = services
            .Count(sd => sd.ServiceType == typeof(IConfigureOptions<RepositoryOptions>));

        Assert.Equal(1, repositoryOptionsRegistrationCount);
    }

}
