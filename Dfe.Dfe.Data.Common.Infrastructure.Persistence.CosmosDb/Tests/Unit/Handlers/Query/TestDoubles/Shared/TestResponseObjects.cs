namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Unit.Handlers.Query.TestDoubles.Shared;

/// <summary>
/// Provides a factory for creating a collection of <see cref="TestResponseObject"/> instances.
/// </summary>
internal static class TestResponseObjects
{
    /// <summary>
    /// Factory method for creating a collection of <see cref="TestResponseObject"/> instances.
    /// </summary>
    /// <returns>
    /// A configured collection of <see cref="TestResponseObject"/> objects.
    /// </returns>
    public static List<TestResponseObject> Create(int totalCount)
    {
        var faker = new Bogus.Faker();
        int amount = faker.Random.Number(1, totalCount);
        var testResponseObjects = new List<TestResponseObject>();

        for (int i = 0; i < amount; i++)
        {
            testResponseObjects.Add(
                TestResponseObject.Create((i+1).ToString(), faker.Name.LastName()));
        }

        return testResponseObjects;
    }
}
