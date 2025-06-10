using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Handlers.Command;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration.Fixture;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration.Fixture.Model;
using FluentAssertions;
using Microsoft.Azure.Cosmos;
using System.Net;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration;

[Collection(CommandIntegrationTestCollection.Name)]
public sealed class CommandHandlerTests
{
    private readonly CosmosDbHandlerFixture<ICosmosDbCommandHandler> _fixture;

    public CommandHandlerTests(CosmosDbHandlerFixture<ICosmosDbCommandHandler> fixture)
    {
        _fixture = fixture;
        ContainerName = fixture.ContainerName;
    }

    private string ContainerName { get; }

    [Fact]
    public async Task CreateItemAsync_CreatesItemAndReturnsResult()
    {
        // arrange
        string itemKey = Guid.NewGuid().ToString();

        // act
        ContainerRecord? result =
            await _fixture.Handler.CreateItemAsync<ContainerRecord>(
                item: ContainerRecord.Create(id: itemKey, username: new Bogus.Faker().Name.FullName()),
                containerKey: ContainerName,
                partitionKeyValue: itemKey);

        // assert
        result.Should().NotBeNull().And.BeAssignableTo<ContainerRecord>();

        await Task.Delay(1000);
    }

    [Fact]
    public async Task UpdateItemAsync_UpdatesItemAndReturnsResult()
    {
        // arrange
        IReadOnlyCollection<ContainerRecord>? containerRecords = _fixture.ContainerRecords;

        var faker = new Bogus.Faker();
        // Reference an existing record.
        string itemKey = containerRecords!.ElementAt(faker.Random.Number(10, 100)).id;
        string updatedUsername = faker.Name.FullName();

        // act
        ContainerRecord? result =
            await _fixture.Handler.UpdateItemAsync<ContainerRecord>(
                item: ContainerRecord.Create(id: itemKey, username: updatedUsername),
                containerKey: ContainerName,
                partitionKeyValue: itemKey);

        // assert
        result.Should().NotBeNull().And.BeAssignableTo<ContainerRecord>();
        result.username.Should().Be(updatedUsername);
        result.id.Should().Be(itemKey);

        await Task.Delay(1000);
    }

    [Fact]
    public async Task DeleteItemAsync_DeletesItemSuccessfully()
    {
        // arrange
        IReadOnlyCollection<ContainerRecord>? containerRecords = _fixture.ContainerRecords;

        var faker = new Bogus.Faker();
        // Reference an existing record.
        string itemKey = containerRecords!.ElementAt(faker.Random.Number(10, 100)).id;

        // assert
        await _fixture.Handler.DeleteItemAsync<ContainerRecord>(
            id: itemKey,
            containerKey: ContainerName,
            partitionKeyValue: itemKey);

        await Task.Delay(1000);

        // Act - Try retrieving the deleted item
        Func<Task> getDeletedItem = async () =>
            await _fixture.Handler.DeleteItemAsync<ContainerRecord>(
            id: itemKey,
            containerKey: ContainerName,
            partitionKeyValue: itemKey);

        // Assert - Confirm item no longer exists (throws NotFound)
        await getDeletedItem.Should().ThrowAsync<CosmosException>()
            .Where(ex => ex.StatusCode == HttpStatusCode.NotFound);
    }
}
