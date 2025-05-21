using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Handlers.Query;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Options;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration.Fixture;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration.Fixture.Configuration;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration.Fixture.Model;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration.Fixture.ServiceProviders;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration;

public sealed class QueryHandlerTests :
    IClassFixture<CosmosDbContextFixture>,
    IClassFixture<CompositionRootServiceProvider>,
    IClassFixture<ConfigurationSettingsBuilder>
{
    private readonly ICosmosDbQueryHandler _queryHandler;

    public QueryHandlerTests(
        CosmosDbContextFixture cosmosDbContextFixture,
        CompositionRootServiceProvider compositionRootServiceProvider,
        ConfigurationSettingsBuilder configurationSettingsBuilder)
    {
        IConfiguration configurationSettings =
            configurationSettingsBuilder.SetupConfiguration(CosmosDbConfigurationStub.GetEmulatorConfiguration());
        IServiceProvider testServiceProvider =
            compositionRootServiceProvider.SetUpServiceProvider(configurationSettings);
        IOptions<RepositoryOptions> repositoryOptions =
            testServiceProvider.GetRequiredService<IOptions<RepositoryOptions>>();
        IReadOnlyCollection<ContainerRecord> containerRecords =
            cosmosDbContextFixture.InitialiseContainerRecords(numberOfRecords: 100);
        Task.Run(
            async () => {
                await cosmosDbContextFixture.Context.CreateAndPopulate(
                    repositoryOptions.Value,
                    containerRecords);
            })
            .GetAwaiter()
            .GetResult();

        _queryHandler = testServiceProvider.GetRequiredService<ICosmosDbQueryHandler>();
    }

    [Fact]
    public async Task ReadItemsAsync_ContainerRecordsAndValidQuery_ReturnsCorrectResults()
    {
        // act
        IEnumerable<ContainerRecord>? results =
            await _queryHandler?.ReadItemsAsync<ContainerRecord>("test-container", "SELECT TOP 10 * FROM c")!;

        // assert
        results.Should().NotBeNullOrEmpty()
            .And.HaveCount(10)
            .And.AllBeAssignableTo<ContainerRecord>();
    }
}
