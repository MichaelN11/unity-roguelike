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
    private const string EntitiesLabel = "entities";
    private const string ItemsLabel = "items";

    private readonly Dictionary<string, Entity> loadedEntities = new();
    private readonly Dictionary<string, Item> loadedItems = new();

    public Entity RetrieveEntity(string name)
    {
        return loadedEntities.GetValueOrDefault(name, null);
    }

    public Item RetrieveItem(string name)
    {
        return loadedItems.GetValueOrDefault(name, null);
    }

    public IEnumerator Load()
    {
        yield return LoadEntities();
        yield return LoadItems();
    }

    private IEnumerator LoadEntities()
    {
        AsyncOperationHandle<IList<Entity>> asyncLoad =
            Addressables.LoadAssetsAsync<Entity>(EntitiesLabel, null);
        yield return asyncLoad;
        foreach (Entity entity in asyncLoad.Result)
        {
            loadedEntities.Add(entity.name, entity);
        }
    }

    private IEnumerator LoadItems()
    {
        AsyncOperationHandle<IList<Item>> asyncLoad =
            Addressables.LoadAssetsAsync<Item>(ItemsLabel, null);
        yield return asyncLoad;
        foreach (Item item in asyncLoad.Result)
        {
            loadedItems.Add(item.name, item);
        }
    }
}
