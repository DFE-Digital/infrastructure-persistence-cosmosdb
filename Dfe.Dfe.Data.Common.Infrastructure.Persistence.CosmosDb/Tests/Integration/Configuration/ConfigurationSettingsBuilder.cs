using Microsoft.Extensions.Configuration;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration.Configuration;

public sealed class ConfigurationSettingsBuilder
{
    public IConfiguration SetupConfiguration(IDictionary<string, string?> configurationSettings)
    {
        ConfigurationBuilder configBuilder = new();

        configBuilder.AddInMemoryCollection(configurationSettings);
        return configBuilder.Build();
    }
}
