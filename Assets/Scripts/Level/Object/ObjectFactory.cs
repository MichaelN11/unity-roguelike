using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Factory class for creating new object instances.
/// </summary>
public class ObjectFactory
{
    public const string ChestType = "Chest";

    public static void LoadObject(ObjectSave objectSave)
    {
        if (objectSave.Type == ChestType)
        {
            GameObject newObject = GameObject.Instantiate(ResourceManager.Instance.ChestObject, objectSave.Position, Quaternion.identity);
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
