using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Handlers.Command;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration.Fixture;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration.Fixture.Model;
using FluentAssertions;
using Microsoft.Azure.Cosmos;
using System.Net;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration;

public sealed class CommandHandlerTests
{
    [Fact]
    public async Task CreateItemAsync_CreatesItemAndReturnsResult()
    {
        var (commandHandler, context, _) =
            await new CosmosDbTestHelper().CreateIsolatedHandlerAsync<ICosmosDbCommandHandler>();

        try
        {
            string itemKey = Guid.NewGuid().ToString();

            var result = await commandHandler.CreateItemAsync<ContainerRecord>(
                item: ContainerRecord.Create(id: itemKey, username: "Test User"),
                containerKey: "test-container",
                partitionKeyValue: itemKey);

            result.Should().NotBeNull().And.BeAssignableTo<ContainerRecord>();
        }
        finally
        {
            context.CleanUpResources();
        }
    }

    [Fact]
    public async Task UpdateItemAsync_UpdatesItemAndReturnsResult()
    {
        CosmosDbTestHelper cosmosDbTestHelper = new();

        var (commandHandler, context, _) =
            await cosmosDbTestHelper.CreateIsolatedHandlerAsync<ICosmosDbCommandHandler>();

        IReadOnlyCollection<ContainerRecord>? containerRecords = cosmosDbTestHelper.ContainerRecords;

        try
        {
            // Reference an existing record.
            string itemKey = containerRecords!.First().id;
            const string updatedUsername = "Updated Test User";

            var result = await commandHandler.UpdateItemAsync<ContainerRecord>(
                item: ContainerRecord.Create(id: itemKey, username: updatedUsername),
                containerKey: "test-container",
                partitionKeyValue: itemKey);

            result.Should().NotBeNull().And.BeAssignableTo<ContainerRecord>();
            result.username.Should().Be(updatedUsername);
            result.id.Should().Be(itemKey);
        }
        finally
        {
            context.CleanUpResources();
        }
    }

    [Fact]
    public async Task DeleteItemAsync_DeletesItemSuccessfully()
    {
        CosmosDbTestHelper cosmosDbTestHelper = new();

        var (commandHandler, context, _) =
            await cosmosDbTestHelper.CreateIsolatedHandlerAsync<ICosmosDbCommandHandler>();

        IReadOnlyCollection<ContainerRecord>? containerRecords = cosmosDbTestHelper.ContainerRecords;

        try
        {
            // Reference an existing record.
            string itemKey = containerRecords!.First().id;

            await commandHandler.DeleteItemAsync<ContainerRecord>(
                id: itemKey,
                containerKey: "test-container",
                partitionKeyValue: itemKey);

            // Check for successful deletion by attempting to update.
            await commandHandler.DeleteItemAsync<ContainerRecord>(
                id: itemKey,
                containerKey: "test-container",
                partitionKeyValue: itemKey);

        }
        catch (CosmosException ex)
        {
            // Check if the exception is due to the item not being found.
            ex.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        finally
        {
            context.CleanUpResources();
        }
    }
}
