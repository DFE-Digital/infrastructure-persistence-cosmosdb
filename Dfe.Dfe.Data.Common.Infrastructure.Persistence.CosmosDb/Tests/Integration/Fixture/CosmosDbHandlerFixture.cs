using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Handlers.Command;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Handlers.Query;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration.Fixture.Model;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration.Fixture.ServiceProviders;
using DfE.GIAP.Core.IntegrationTests.Fixture;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration.Fixture
{
    /// <summary>
    /// Fixture class for setting up a Cosmos DB handler for integration tests.
    /// </summary>
    /// <typeparam name="THandlerType">The type of handler to incorporate,
    /// i.e. a <see cref="ICosmosDbQueryHandler"/> or <see cref="ICosmosDbCommandHandler"/>.
    /// </typeparam>
    public class CosmosDbHandlerFixture<THandlerType> where THandlerType : class
    {
        /// <summary>
        /// Public property exposing the query handler used to interact with Cosmos DB.
        /// </summary>
        public THandlerType? Handler { get; private set; }

        /// <summary>
        /// A collection of container records used for testing purposes.
        /// </summary>
        public IReadOnlyCollection<ContainerRecord>? ContainerRecords { get; private set; }

        /// <summary>
        /// The name of the Cosmos DB container used for testing.
        /// </summary>
        public string ContainerName { get; private set; } = string.Empty;

        /// <summary>
        /// The Cosmos DB context used for creating and managing the test database and container.
        /// </summary>
        private CosmosDbFixture DbFixture { get; }

        /// <summary>
        /// The partition key path used for the container.
        /// </summary>
        private const string partitionKeyPath = "/pk";

        /// <summary>
        /// Initializes a new instance of the <see cref="CosmosDbHandlerFixture{THandlerType}"/> class.
        /// </summary>
        public CosmosDbHandlerFixture()
        {
            DbFixture = new CosmosDbFixture();
            Task.Run(() => DbFixture.InitializeAsync()).GetAwaiter().GetResult();
            Task.Run(() => InitializeAsync("integration-test-container")).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Initializes the Cosmos DB test fixture, creating an isolated query handler for testing.
        /// </summary>
        public async Task InitializeAsync(string containerName, int numberOfRecords = 100)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(containerName);

            ContainerName = containerName;

            // Build configuration using stubbed emulator settings.
            IConfiguration configurationSettings =
                new ConfigurationSettingsBuilder()
                    .SetupConfiguration(CosmosDbConfigurationStub.GetEmulatorConfiguration(containerName));

            // Set up the service provider with all required services and dependencies.
            IServiceProvider testServiceProvider =
                new CompositionRootServiceProvider()
                    .ConfigureServiceProvider(configurationSettings);

            // Generate a collection of fake container records for testing.
            ContainerRecords = InitialiseContainerRecords(numberOfRecords);

            // Create the test database and container(s).
            await DbFixture.InitializeAsync();
            await DbFixture.Database.StartAsync(partitionKeyPath: partitionKeyPath);

            // Add the test records.
            foreach (ContainerRecord? containerRecord in ContainerRecords)
            {
                await DbFixture.Database.WriteAsync(containerRecord, partitionKeyPath);
            }

            // Resolve the Cosmos DB handler from the service provider.
            Handler = testServiceProvider.GetRequiredService<THandlerType>();
        }

        /// <summary>
        /// Generates a collection of fake container records using the Bogus library.
        /// </summary>
        /// <param name="numberOfRecords">The number of records to generate.</param>
        /// <returns>A read-only collection of generated container records.</returns>
        private IReadOnlyCollection<ContainerRecord> InitialiseContainerRecords(int numberOfRecords) =>
            new Bogus.Faker<ContainerRecord>()
                .StrictMode(true) // Ensures all properties must be explicitly defined.
                .RuleFor(containerRecord => containerRecord.id, _ => Guid.NewGuid().ToString())             // Unique ID.
                .RuleFor(containerRecord => containerRecord.username, fake => fake.Internet.UserName())     // Fake username.
                .RuleFor(containerRecord => containerRecord.pk, (_, containerRecord) => containerRecord.id) // Partition key = ID.
                .Generate(numberOfRecords);
    }
}
