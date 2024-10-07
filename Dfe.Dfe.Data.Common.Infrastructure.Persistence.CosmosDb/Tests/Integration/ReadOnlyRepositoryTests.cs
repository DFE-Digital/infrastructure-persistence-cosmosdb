using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Options;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Repositories;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration.Configuration;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration.Model;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration.ServiceProviders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using Microsoft.Extensions.Options;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration
{
    public sealed class ReadOnlyRepositoryTests :
        IClassFixture<CosmosDbContextFixture>,
        IClassFixture<CompositionRootServiceProvider>,
        IClassFixture<ConfigurationSettingsBuilder>
    {
        private readonly IConfiguration _configurationSettings;
        private readonly IServiceProvider _compositionRootServiceProvider;
        private readonly CosmosDbContextFixture _cosmosDbContextFixture;

        private static Dictionary<string, string?> InMemoryConfiguration => new(){
            { "RepositoryOptions:EndpointUri", "https://localhost:8081" },
            { "RepositoryOptions:PrimaryKey", "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw=="},
            { "RepositoryOptions:DatabaseId", "integration-test"},
            { "RepositoryOptions:ConnectionMode", "0"},
            { "RepositoryOptions:Containers:0:test-container:ContainerName", "test-container"},
            { "RepositoryOptions:Containers:0:test-container:PartitionKey", "/pk"},
        };

        public ReadOnlyRepositoryTests(
            CosmosDbContextFixture cosmosDbContextFixture,
            CompositionRootServiceProvider compositionRootServiceProvider,
            ConfigurationSettingsBuilder configurationSettingsBuilder)
        {
            _cosmosDbContextFixture = cosmosDbContextFixture;
            _configurationSettings =
                configurationSettingsBuilder.SetupConfiguration(InMemoryConfiguration);
            _compositionRootServiceProvider =
                compositionRootServiceProvider.SetUpServiceProvider(_configurationSettings);
        }

        [Fact]
        public async Task GetAllItemsByQueryAsync_ContainerRecordsAndValidQuery_ReturnsCorrectResults()
        {
            // arrange
            IOptions<RepositoryOptions> repositoryOptions =
                _compositionRootServiceProvider.GetRequiredService<IOptions<RepositoryOptions>>();

            IReadOnlyCollection<ContainerRecord> containerRecords =
                _cosmosDbContextFixture.InitialiseContainerRecords(numberOfRecords: 1000);

            await _cosmosDbContextFixture.Context.CreateAndPopulate(repositoryOptions.Value, containerRecords);

            IReadOnlyRepository readOnlyRepository =
                _compositionRootServiceProvider.GetRequiredService<IReadOnlyRepository>();

            // act
            IEnumerable<ContainerRecord>? results =
                await readOnlyRepository?
                    .GetAllItemsByQueryAsync<ContainerRecord>("SELECT TOP 10 * FROM c", "test-container")!;

            // assert
            results.Should().NotBeNullOrEmpty()
                .And.HaveCount(10)
                .And.AllBeAssignableTo<ContainerRecord>();
        }
    }
}
