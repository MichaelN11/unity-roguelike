using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <summary>
/// Service class for loading game data using Unity's Addressables.
/// </summary>
public class AddressableService
{
    private const string EntitiesLabel = "default";

    private readonly Dictionary<string, Entity> loadedEntities = new();

    public IEnumerator LoadEntities()
    {
        AsyncOperationHandle<IList<Entity>> asyncLoad =
            Addressables.LoadAssetsAsync<Entity>(EntitiesLabel, null);
        yield return asyncLoad;
        foreach (Entity entity in asyncLoad.Result)
        {
            loadedEntities.Add(entity.name, entity);
        }
    }

    public Entity RetrieveEntity(string name)
    {
        return loadedEntities.GetValueOrDefault(name, null);
    }
}
