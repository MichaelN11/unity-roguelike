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
        if (inventoryItems != null)
        {
            foreach (InventoryItem item in inventoryItems)
            {
                InventoryItem newItem = new();
                newItem.Item = item.Item;
                newItem.Amount = item.Amount;
                inventory.Items.Add(newItem);
            }
        }
        return inventory;
    }

    public bool AcquireItem(InventoryItem inventoryItem)
    {
        if (inventoryItem.Item.UseOnPickup)
        {
            return UseAbility(inventoryItem, Vector2.zero, 0);
        }
        else
        {
            AddItem(inventoryItem.Item, inventoryItem.Amount);
            return true;
        }
    }

    public bool AcquireItem(Item item, int amount)
    {
        InventoryItem inventoryItem = new()
        {
            Item = item,
            Amount = amount
        };
        return AcquireItem(inventoryItem);
    }

    /// <summary>
    /// Uses an item in the inventory based off the item's number and the direction.
    /// The offset distance is the distance the item's effect should be offset from the entity.
    /// </summary>
    /// <param name="itemNumber"></param>
    /// <param name="direction"></param>
    /// <param name="offsetDistance"></param>
    /// <returns></returns>
    public bool UseItemFromInventory(int itemNumber, Vector2 direction, float offsetDistance)
    {
        bool success = false;
        if (itemNumber >= 0
            && itemNumber < Items.Count
            && abilityManager != null)
        {
            InventoryItem inventoryItem = Items[itemNumber];
            if (inventoryItem.Amount > 0)
            {
                success = UseAbility(inventoryItem, direction, offsetDistance); ;
                if (success)
                {
                    --inventoryItem.Amount;
                }
            }
        }
        return success;
    }

    private void AddItem(Item item, int amount)
    {
        InventoryItem inventoryItem = Items.Find(it => it.Item.name == item.name);
        if (inventoryItem != null)
        {
            inventoryItem.Amount += amount;
        }
        else
        {
            inventoryItem = new();
            inventoryItem.Item = item;
            inventoryItem.Amount = amount;
            Items.Add(inventoryItem);
        }
    }

    private bool UseAbility(InventoryItem inventoryItem, Vector2 direction, float offsetDistance)
    {
        ActiveAbilityContext abilityContext = new()
        {
            Ability = inventoryItem.Item.ActiveAbility
        };
        AbilityUseEventInfo abilityUseEvent = abilityManager.UseAbility
            (abilityContext, direction, offsetDistance);
        bool success = abilityUseEvent != null;
        if (success)
        {
            OnItemUse?.Invoke(new ItemUseEventInfo() { Item = inventoryItem.Item, AbilityUseEventInfo = abilityUseEvent });
        }
        return success;
    }
}
