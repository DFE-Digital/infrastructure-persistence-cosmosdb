using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration.Model;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration;

/// <summary>
/// 
/// </summary>
public class CosmosDbContextFixture : IDisposable
{
    /// <summary>
    /// 
    /// </summary>
    public CosmosDbContextFixture()
    {
        Context = new CosmosDbContext();
    }

    /// <summary>
    /// 
    /// </summary>

    // To detect redundant calls
    private bool _contextDisposed;

    /// <summary>
    /// 
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="disposing"></param>
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

    /// <summary>
    /// /
    /// </summary>
    public CosmosDbContext Context { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
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
