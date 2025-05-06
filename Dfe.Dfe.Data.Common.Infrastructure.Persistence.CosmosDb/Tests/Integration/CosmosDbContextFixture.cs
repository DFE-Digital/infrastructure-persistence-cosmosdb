using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration.Model;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration;

public class CosmosDbContextFixture : IDisposable
{
    public CosmosDbContextFixture()
    {
        Context = new CosmosDbContext();
    }

    private bool _contextDisposed;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_contextDisposed)
        {
            if (disposing)
            {
                Context.CleanUpResources();
            }
            _contextDisposed = true;
        }
    }

    public CosmosDbContext Context { get; }

    public IReadOnlyCollection<ContainerRecord> InitialiseContainerRecords(int numberOfRecords) =>
        new Bogus.Faker<ContainerRecord>()
            .StrictMode(true)
            .RuleFor(containerRecord =>
                containerRecord.id, _ => Guid.NewGuid().ToString())
            .RuleFor(containerRecord =>
                containerRecord.username, fake => fake.Internet.UserName())
            .RuleFor(containerRecord => containerRecord.pk, (_, containerRecord) => containerRecord.id) //partition key
            .Generate(numberOfRecords);
}
