using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Handlers.Command;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration.Fixture;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration.Fixture.Model;
using FluentAssertions;

public sealed class CommandHandlerTests
{
    [Fact]
    public async Task CreateItemAsync_CreatesItemAndReturnsResult()
    {
        //var (commandHandler, context, _) =
        //    await new CosmosDbTestHelper().CreateIsolatedHandlerAsync<ICosmosDbCommandHandler>();

        //try
        //{
        //    string itemKey = Guid.NewGuid().ToString();

        //    var result = await commandHandler.CreateItemAsync<ContainerRecord>(
        //        item: ContainerRecord.Create(itemKey, "Test User"),
        //        containerKey: "test-container",
        //        partitionKeyValue: itemKey);

        //    result.Should().NotBeNull()
        //        .And.BeAssignableTo<ContainerRecord>();
        //}
        //finally
        //{
        //    context.CleanUpResources();
        //}
    }
}
