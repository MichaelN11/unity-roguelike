using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component for managing an entity's inventory.
/// </summary>
public class Inventory : MonoBehaviour
{
    public List<InventoryItem> Items { get; set; } = new();

    /// <summary>
    /// The event that fires when an item is used by the entity.
    /// </summary>
    public event Action<ItemUseEventInfo> OnItemUse;

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

    public void AddItem(Item item, int amount)
    {
        InventoryItem inventoryItem = Items.Find(it => it.Item.name == item.name);
        if (inventoryItem != null)
        {
            inventoryItem.Amount += amount;
        } else
        {
            inventoryItem = new();
            inventoryItem.Item = item;
            inventoryItem.Amount = amount;
            Items.Add(inventoryItem);
        }
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
                AbilityUseEventInfo abilityUseEvent = abilityManager.UseAbility
                    (inventoryItem.Item.ActiveAbility, direction, offsetDistance);
                success = abilityUseEvent != null;
                if (success)
                {
                    --inventoryItem.Amount;
                    OnItemUse?.Invoke(new ItemUseEventInfo(){ Item = inventoryItem.Item, AbilityUseEventInfo = abilityUseEvent });
                }
            }
        }
        return success;
    }
}
