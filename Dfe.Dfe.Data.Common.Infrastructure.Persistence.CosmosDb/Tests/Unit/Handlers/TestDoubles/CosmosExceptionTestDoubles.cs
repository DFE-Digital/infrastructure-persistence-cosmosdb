using Microsoft.Azure.Cosmos;
using Moq;
using System.Net;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Tests.Unit.Handlers.TestDoubles;
internal static class CosmosExceptionTestDoubles
{
    internal static CosmosException Default()
        => new(
            It.IsAny<string>(),
            It.IsAny<HttpStatusCode>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<double>());

    internal static CosmosException WithStatusCode(HttpStatusCode code) => new(
        It.IsAny<string>(),
        code,
        It.IsAny<int>(),
        It.IsAny<string>(),
        It.IsAny<double>());
}
