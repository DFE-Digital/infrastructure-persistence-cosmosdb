// Ignore Spelling: Username

using System.Diagnostics.CodeAnalysis;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration.Model
{
    [SuppressMessage("Microsoft.Performance", "CD1600: The class must have a documentation header.")]
    [SuppressMessage("Microsoft.Performance", "ClassDocumentationHeader: The class must have a documentation header.")]
    public sealed class ContainerRecord : IContainerRecord
    {
        [SuppressMessage("Microsoft.Performance", "CD1606: The property must have a documentation header.")]
        [SuppressMessage("Microsoft.Performance", "PropertyDocumentationHeader: The property must have a documentation header.")]
        public string id { get; set; } = string.Empty;

        [SuppressMessage("Microsoft.Performance", "CD1606: The property must have a documentation header.")]
        [SuppressMessage("Microsoft.Performance", "PropertyDocumentationHeader: The property must have a documentation header.")]
        public string pk { get; set; } = string.Empty;


        [SuppressMessage("Microsoft.Performance", "CD1606: The property must have a documentation header.")]
        [SuppressMessage("Microsoft.Performance", "PropertyDocumentationHeader: The property must have a documentation header.")]
        public string username { get; set; } = string.Empty;
    }
}
