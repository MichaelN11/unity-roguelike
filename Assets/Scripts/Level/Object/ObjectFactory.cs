using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Factory class for creating new object instances.
/// </summary>
public class ObjectFactory
{
    public const string ChestType = "Chest";

    public static GameObject CreateObject(string name, Vector2 position)
    {
        GameObject objectPrefab = GameManager.Instance.AddressableService.RetrieveObject(name);
        return Object.Instantiate(objectPrefab, position, Quaternion.identity);
    }

    public static Chest CreateChest(Vector2 position, ItemDrop itemDrop)
    {
        GameObject newObject = CreateObject("Chest", position);
        if (newObject != null && newObject.TryGetComponent<Chest>(out Chest chest))
        {
            chest.containedItem.Item = itemDrop.Item;
            chest.containedItem.Amount = itemDrop.Amount;
            return chest;
        }
        else
        {
            Debug.Log("Addressable chest object is null or does not have Chest component.");
            return null;
        }
    }

    public static void LoadObject(ObjectSave objectSave)
    {
        GameObject newObject = CreateObject(objectSave.Type, objectSave.Position);

        if (objectSave.Type == ChestType)
        {
            Chest chest = newObject.GetComponent<Chest>();
            if (objectSave.InventoryItem != null)
            {
                chest.containedItem.Item = GameManager.Instance.AddressableService.RetrieveItem(objectSave.InventoryItem.Name);
                chest.containedItem.Amount = objectSave.InventoryItem.Amount;
            } else
            {
                chest.SetToOpen();
            }
        }
    }
}
