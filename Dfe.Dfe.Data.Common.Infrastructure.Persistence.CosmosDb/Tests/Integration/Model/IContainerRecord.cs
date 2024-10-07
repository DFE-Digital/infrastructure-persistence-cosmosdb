using System.Diagnostics.CodeAnalysis;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration.Model
{
    [SuppressMessage("Microsoft.Performance", "InterfaceDocumentationHeader: The interface must have a documentation header.")]
    public interface IContainerRecord
    {
        [SuppressMessage("Microsoft.Performance", "PropertyDocumentationHeader: The property must have a documentation header.")]
        string id { get; set; }
    }
}