namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Integration.Model
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ContainerRecord : IContainerRecord
    {
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string pk { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string username { get; set; } = string.Empty;
    }
}
