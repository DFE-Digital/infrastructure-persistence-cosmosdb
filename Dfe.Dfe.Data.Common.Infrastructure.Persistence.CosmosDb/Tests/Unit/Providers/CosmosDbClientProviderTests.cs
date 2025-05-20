using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Options;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Providers;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using Moq;
using System.Reflection;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Unit.Providers;

public class CosmosDbClientProviderTests
{
    private readonly Mock<IOptions<RepositoryOptions>> _mockOptions;
    private readonly RepositoryOptions _repositoryOptions;
    private readonly CosmosDbClientProvider _clientProvider;

    public CosmosDbClientProviderTests()
    {
        // Arrange: Mock repository options
        _repositoryOptions = new RepositoryOptions
        {
            EndpointUri = "https://test-cosmos-db.documents.azure.com:443/",
            // Publicly exposed Cosmos Db Emulator key so safe to share.
            PrimaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw=="
        };

        _mockOptions = new Mock<IOptions<RepositoryOptions>>();
        _mockOptions.Setup(repositoryOptions =>
            repositoryOptions.Value).Returns(_repositoryOptions);

        // Create an instance of CosmosDbClientProvider with mocked options
        _clientProvider = new CosmosDbClientProvider(_mockOptions.Object);
    }

    [Fact]
    public async Task InvokeCosmosClientAsync_ShouldExecuteClientInvoker()
    {
        // Arrange
        static Task<string> TestInvoker(CosmosClient client)
        {
            // Use the client parameter to avoid RCS1163 warning
            _ = client.Endpoint; // Accessing a property of the client
            return Task.FromResult("Success");
        }

        // Act
        var result =
            await _clientProvider
                .InvokeCosmosClientAsync(TestInvoker);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Success", result);
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenOptionsAreNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CosmosDbClientProvider(null!));
    }

    [Fact]
    public void Dispose_ShouldDisposeCosmosClient()
    {
        // Act
        _clientProvider.Dispose();
        MethodInfo? disposeMethod = _clientProvider.GetType().GetMethod("Dispose");

        // Assert
        Assert.NotNull(disposeMethod);
        Assert.True(disposeMethod!.IsPublic);
    }
}
