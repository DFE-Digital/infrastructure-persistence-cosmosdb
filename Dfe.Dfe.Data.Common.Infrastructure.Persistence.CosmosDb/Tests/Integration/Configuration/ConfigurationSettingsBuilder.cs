using Microsoft.Extensions.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration.Configuration
{
    [SuppressMessage("Microsoft.Performance", "CD1600: The class must have a documentation header.")]
    [SuppressMessage("Microsoft.Performance", "ClassDocumentationHeader: The class must have a documentation header.")]
    public sealed class ConfigurationSettingsBuilder
    {
        [SuppressMessage("Microsoft.Performance", "CD1605: The method must have a documentation header.")]
        [SuppressMessage("Microsoft.Performance", "MethodDocumentationHeader: The method must have a documentation header.")]
        public IConfiguration SetupConfiguration(IDictionary<string, string?> configurationSettings)
        {
            ConfigurationBuilder configBuilder = new();

            configBuilder.AddInMemoryCollection(configurationSettings);
            return configBuilder.Build();
        }
    }
}
