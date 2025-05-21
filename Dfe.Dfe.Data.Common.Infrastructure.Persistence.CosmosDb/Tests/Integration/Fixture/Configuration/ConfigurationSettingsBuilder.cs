using Microsoft.Extensions.Configuration;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration.Fixture.Configuration;

/// <summary>
/// Responsible for building configuration settings for integration tests.
/// </summary>
public sealed class ConfigurationSettingsBuilder
{
    /// <summary>
    /// Sets up an in-memory configuration using the provided key-value pairs.
    /// </summary>
    /// <param name="configurationSettings">A dictionary containing configuration keys and their corresponding values.</param>
    /// <returns>An IConfiguration object built from the in-memory collection.</returns>
    public IConfiguration SetupConfiguration(IDictionary<string, string?> configurationSettings)
    {
        // Create a new instance of ConfigurationBuilder
        ConfigurationBuilder configBuilder = new();

        // Add the provided settings to the configuration builder as an in-memory collection
        configBuilder.AddInMemoryCollection(configurationSettings);

        // Build and return the IConfiguration object
        return configBuilder.Build();
    }
}