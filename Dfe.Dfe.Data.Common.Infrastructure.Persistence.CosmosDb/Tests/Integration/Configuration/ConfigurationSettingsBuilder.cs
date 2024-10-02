using Microsoft.Extensions.Configuration;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ConfigurationSettingsBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configurationSettings"></param>
        /// <returns></returns>
        public IConfiguration SetupConfiguration(IDictionary<string, string?> configurationSettings)
        {
            ConfigurationBuilder configBuilder = new();

            configBuilder.AddInMemoryCollection(configurationSettings);
            return configBuilder.Build();
        }
    }
}
