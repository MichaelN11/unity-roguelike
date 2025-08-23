using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Factory class for creating new object instances.
/// </summary>
public class ObjectFactory
{
    public static GameObject CreateObject(string name, Vector2 position, InventoryItem inventoryItem = null)
    {
        GameObject objectPrefab = GameManager.Instance.AddressableService.RetrieveObject(name);
        return CreateObject(objectPrefab, position, inventoryItem);
    }

    public static GameObject CreateObject(GameObject objectPrefab, Vector2 position, InventoryItem inventoryItem = null)
    {
        GameObject newObject = Object.Instantiate(objectPrefab, position, Quaternion.identity);

        if (newObject != null && newObject.TryGetComponent<LevelObject>(out LevelObject levelObject))
        {
            levelObject.Type = objectPrefab.name;
            if (inventoryItem != null)
            {
                levelObject.ContainedItem = inventoryItem;
            }
        }
        else
        {
            Debug.LogWarning("Addressable level object is null or does not have LevelObject component.");
        }
        return newObject;
    }

    public static void LoadObject(ObjectSave objectSave)
    {
        InventoryItemSave itemSave = objectSave.InventoryItem;
        if (itemSave != null)
        {
            InventoryItem inventoryItem = ItemFactory.LoadItem(itemSave);
            CreateObject(objectSave.Type, objectSave.Position, inventoryItem);
        } else
        {
            CreateObject(objectSave.Type, objectSave.Position);
        }
    }
}
