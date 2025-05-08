using Castle.Core.Logging;
using Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Providers;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Unit.Providers.TestDoubles
{
    /// <summary>
    /// Provides test doubles for Microsoft.Extensions.Logging.ILogger.
    /// </summary>
    internal static class LoggerTestDouble
    {
        /// <summary>
        /// Creates a default mock of <see cref="ILogger"/>.
        /// </summary>
        /// <returns>A new Mock<ILogger> instance.</returns>
        public static Mock<ILogger<CosmosDbContainerProvider>> DefaultMock() => new();

        /// <summary>
        /// Creates a mock of <see cref="ILogger"/>.
        /// </summary>
        /// <returns>A configured Mock<ILogger> instance.</returns>
        public static Mock<ILogger<CosmosDbContainerProvider>> MockFor(
            Expression<Action<ILogger<CosmosDbContainerProvider>>> logAction)
        {
            // Create a default mock of the logger.
            Mock<ILogger<CosmosDbContainerProvider>> mockLogger = DefaultMock();

            // Setup the mock to setup the log for verification.
            mockLogger.Setup(logAction).Verifiable();

            return mockLogger;
        }
    }
}
