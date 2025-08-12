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
    private const string ObjectLabel = "objects";
    private const string AbilityLabel = "abilities";

    private readonly Dictionary<string, Entity> loadedEntities = new();
    private readonly Dictionary<string, Item> loadedItems = new();
    private readonly Dictionary<string, GameObject> loadedObjects = new();
    private readonly Dictionary<string, ActiveAbility> loadedAbilities = new();

    public Entity RetrieveEntity(string name)
    {
        return loadedEntities.GetValueOrDefault(name, null);
    }

    public Item RetrieveItem(string name)
    {
        return loadedItems.GetValueOrDefault(name, null);
    }

    public GameObject RetrieveObject(string name)
    {
        return loadedObjects.GetValueOrDefault(name, null);
    }

    public ActiveAbility RetrieveAbility(string name)
    {
        return loadedAbilities.GetValueOrDefault(name, null);
    }

    public IEnumerator Load()
    {
        yield return LoadEntities();
        yield return LoadItems();
        yield return LoadObjects();
        yield return LoadAbilities();
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

    private IEnumerator LoadObjects()
    {
        AsyncOperationHandle<IList<GameObject>> asyncLoad =
            Addressables.LoadAssetsAsync<GameObject>(ObjectLabel, null);
        yield return asyncLoad;
        foreach (GameObject gameObject in asyncLoad.Result)
        {
            loadedObjects.Add(gameObject.name, gameObject);
        }
    }

    private IEnumerator LoadAbilities()
    {
        AsyncOperationHandle<IList<ActiveAbility>> asyncLoad =
            Addressables.LoadAssetsAsync<ActiveAbility>(AbilityLabel, null);
        yield return asyncLoad;
        foreach (ActiveAbility ability in asyncLoad.Result)
        {
            loadedAbilities.Add(ability.name, ability);
        }
    }
}
