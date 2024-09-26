namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Options.Extensions;

/// <summary>
/// 
/// </summary>
public static class RepositoryOptionsExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="repositoryOptions"></param>
    /// <param name="containerKey"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static ContainerOptions GetContainerOptions(
        this RepositoryOptions repositoryOptions, string containerKey)
    {
        _ = TryGetContainerOptionsDictionary(repositoryOptions, containerKey)
            .TryGetValue(containerKey, out var container);

        if (container == null){
            throw new InvalidOperationException(
                $"Container dictionary options with container key: {containerKey} not configured in options.");
        }

        return container;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="repositoryOptions"></param>
    /// <param name="containerKey"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static Dictionary<string, ContainerOptions> TryGetContainerOptionsDictionary(
        this RepositoryOptions repositoryOptions, string containerKey) =>
            repositoryOptions.Containers?
                .SingleOrDefault(containerOptionsDict =>
                    containerOptionsDict.ContainsKey(containerKey)) ??
                    throw new InvalidOperationException(
                        $"Container with key: {containerKey} not configured in options.");
}
