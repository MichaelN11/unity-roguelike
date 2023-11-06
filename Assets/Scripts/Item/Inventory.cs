using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component for managing an entity's inventory.
/// </summary>
public class Inventory : MonoBehaviour
{
    private const float itemXOffset = 0.2f;
    private const float itemYOffset = 0.2f;

    public List<InventoryItem> Items { get; set; } = new();

    private AbilityManager abilityManager;
    private EntityState entityState;

    private GameObject usedItemObject = null;

    private void Awake()
    {
        abilityManager = GetComponentInChildren<AbilityManager>();
        entityState = GetComponent<EntityState>();
    }

    private void Update()
    {
        if (usedItemObject != null && entityState.ActionState != ActionState.Ability)
        {
            Destroy(usedItemObject);
            usedItemObject = null;
        }
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
                success = abilityManager.UseAbility(inventoryItem.Item.ActiveAbility, direction, offsetDistance);
                if (success)
                {
                    --inventoryItem.Amount;
                    AnimateItem(inventoryItem.Item, direction);
                }
            }
        }
        return success;
    }

    private void AnimateItem(Item item, Vector2 direction)
    {
        if (item.Prefab != null && !DirectionUtil.IsFacingUp(direction))
        {
            Vector2 itemPosition = new(transform.position.x, transform.position.y + itemYOffset);
            if (DirectionUtil.IsFacingLeft(direction))
            {
                itemPosition.x -= itemXOffset;
            } else if (DirectionUtil.IsFacingRight(direction))
            {
                itemPosition.x += itemXOffset;
            }
            usedItemObject = Instantiate(item.Prefab, itemPosition, Quaternion.identity, transform);
        }
    }
}
