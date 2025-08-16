using System;
using UnityEngine;

/// <summary>
/// Class storing data about an item that can be dropped by an entity.
/// </summary>
[Serializable]
public class ItemDrop
{
    [SerializeField]
    private float dropChance;
    public float DropChance
    {
        get => dropChance;
        set => dropChance = value;
    }

    [SerializeField]
    private InventoryItem inventoryItem = new();
    public InventoryItem InventoryItem
    {
        get => inventoryItem;
        set => inventoryItem = value;
    }

    public ItemDrop Clone()
    {
        return new()
        {
            dropChance = this.dropChance,
            inventoryItem = this.inventoryItem.Clone()
        };
    }
}
