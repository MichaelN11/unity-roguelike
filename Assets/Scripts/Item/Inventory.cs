using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component for managing an entity's inventory.
/// </summary>
public class Inventory : MonoBehaviour
{
    public List<InventoryItem> Items { get; set; } = new();

    private AbilityManager abilityManager;

    private void Awake()
    {
        abilityManager = GetComponentInChildren<AbilityManager>();
    }

    public static Inventory AddToObject(GameObject gameObject, List<InventoryItem> inventoryItems)
    {
        Inventory inventory = gameObject.AddComponent<Inventory>();
        foreach (InventoryItem item in inventoryItems)
        {
            InventoryItem newItem = new();
            newItem.Item = item.Item;
            newItem.Amount = item.Amount;
            inventory.Items.Add(newItem);
        }
        return inventory;
    }

    /// <summary>
    /// Uses an item in the inventory based off the item's number and the direction.
    /// The offset distance is the distance the item's effect should be offset from the entity.
    /// </summary>
    /// <param name="itemNumber"></param>
    /// <param name="direction"></param>
    /// <param name="offsetDistance"></param>
    /// <returns></returns>
    public bool UseItem(int itemNumber, Vector2 direction, float offsetDistance)
    {
        bool success = false;
        if (itemNumber >= 0
            && itemNumber < Items.Count
            && abilityManager != null)
        {
            InventoryItem inventoryItem = Items[itemNumber];
            if (inventoryItem.Amount > 0)
            {
                success = abilityManager.UseAbility(inventoryItem.Item.ActiveAbility, direction, offsetDistance);
                if (success)
                {
                    --inventoryItem.Amount;
                }
            }
        }
        return success;
    }
}