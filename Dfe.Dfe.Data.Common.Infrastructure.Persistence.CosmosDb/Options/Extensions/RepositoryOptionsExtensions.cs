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
        _ = TryGetContainerOptionsDictionary(repositoryOptions, containerKey)
            .TryGetValue(containerKey, out var container);

        // Validate if the container was found.
        if (container == null)
        {
            throw new InvalidOperationException(
                $"Container dictionary options with container key: {containerKey} not configured in options.");
        }

        return container;
    }

    /// <summary>
    /// Retrieves the dictionary of container options from the repository configurations.
    /// </summary>
    /// <param name="repositoryOptions">The repository options containing container configurations.</param>
    /// <param name="containerKey">The key identifying the container.</param>
    /// <returns>A dictionary containing container configurations.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if no container is found for the given key.
    /// </exception>
    public static Dictionary<string, ContainerOptions> TryGetContainerOptionsDictionary(
        this RepositoryOptions repositoryOptions, string containerKey) =>
            repositoryOptions.Containers?
                .SingleOrDefault(containerOptionsDict =>
                    containerOptionsDict.ContainsKey(containerKey)) ??
                    throw new InvalidOperationException(
                        $"Container with key: {containerKey} not configured in options.");
}