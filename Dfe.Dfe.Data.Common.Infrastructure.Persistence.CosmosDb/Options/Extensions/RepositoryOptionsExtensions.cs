namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Options.Extensions;

/// <summary>
/// Provides extension methods for retrieving container options from repository configurations.
/// </summary>
public static class RepositoryOptionsExtensions
{
    /// <summary>
    /// Retrieves container options for a specified container key.
    /// </summary>
    /// <param name="repositoryOptions">The repository options containing container configurations.</param>
    /// <param name="containerKey">The key identifying the container.</param>
    /// <returns>The container options for the specified key.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the container options are not configured for the given key.
    /// </exception>
    public static ContainerOptions GetContainerOptions(
        this RepositoryOptions repositoryOptions, string containerKey)
    {
        // Attempt to retrieve the container options dictionary and fetch the requested container.

        ArgumentNullException.ThrowIfNull(repositoryOptions.Containers);
        if(!repositoryOptions.Containers.TryGetValue(containerKey, out ContainerOptions? containerOptions))
        {
            throw new InvalidOperationException(
                $"Container dictionary options with container key: {containerKey} not configured in options.");
        }

        return containerOptions;
    }
}