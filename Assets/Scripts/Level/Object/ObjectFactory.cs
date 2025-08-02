using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Factory class for creating new object instances.
/// </summary>
public class ObjectFactory
{
    public static GameObject CreateObject(string name, Vector2 position, Item containedItem = null, int itemAmount = 1)
    {
        GameObject objectPrefab = GameManager.Instance.AddressableService.RetrieveObject(name);
        return CreateObject(objectPrefab, position, containedItem, itemAmount);
    }

    public static GameObject CreateObject(GameObject objectPrefab, Vector2 position, Item containedItem = null, int itemAmount = 1)
    {
        GameObject newObject = Object.Instantiate(objectPrefab, position, Quaternion.identity);

        if (newObject != null && newObject.TryGetComponent<LevelObject>(out LevelObject levelObject))
        {
            levelObject.type = objectPrefab.name;
            if (containedItem != null && itemAmount > 0)
            {
                levelObject.containedItem.Item = containedItem;
                levelObject.containedItem.Amount = itemAmount;
            }
        }
        else
        {
            Debug.Log("Addressable level object is null or does not have LevelObject component.");
        }
        return newObject;
    }

    public static void LoadObject(ObjectSave objectSave)
    {
        if (objectSave.InventoryItem != null)
        {
            Item containedItem = GameManager.Instance.AddressableService.RetrieveItem(objectSave.InventoryItem.Name);
            int itemAmount = objectSave.InventoryItem.Amount;
            CreateObject(objectSave.Type, objectSave.Position, containedItem, itemAmount);
        } else
        {
            CreateObject(objectSave.Type, objectSave.Position);
        }
    }
}
