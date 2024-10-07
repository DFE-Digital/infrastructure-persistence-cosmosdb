using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration.Model;
using System.Diagnostics.CodeAnalysis;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration;

[SuppressMessage("Microsoft.Performance", "CD1600: The class must have a documentation header.")]
[SuppressMessage("Microsoft.Performance", "ClassDocumentationHeader: The class must have a documentation header.")]
public class CosmosDbContextFixture : IDisposable
{
    [SuppressMessage("Microsoft.Performance", "ConstructorDocumentationHeader: The constructor must have a documentation header.")]
    public CosmosDbContextFixture()
    {
        Context = new CosmosDbContext();
    }

    private bool _contextDisposed;

    [SuppressMessage("Microsoft.Performance", "CD1605: The method must have a documentation header.")]
    [SuppressMessage("Microsoft.Performance", "MethodDocumentationHeader: The method must have a documentation header.")]
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    [SuppressMessage("Microsoft.Performance", "CD1605: The method must have a documentation header.")]
    [SuppressMessage("Microsoft.Performance", "MethodDocumentationHeader: The method must have a documentation header.")]
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

    [SuppressMessage("Microsoft.Performance", "CD1606: The property must have a documentation header.")]
    [SuppressMessage("Microsoft.Performance", "PropertyDocumentationHeader: The property must have a documentation header.")]
    public CosmosDbContext Context { get; }

    [SuppressMessage("Microsoft.Performance", "CD1605: The method must have a documentation header.")]
    [SuppressMessage("Microsoft.Performance", "MethodDocumentationHeader: The method must have a documentation header.")]
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
