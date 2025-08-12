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
    public float DropChance => dropChance;

    [SerializeField]
    private InventoryItem inventoryItem = new();
    public InventoryItem InventoryItem => inventoryItem;

    public ItemDrop Clone()
    {
        return new()
        {
            dropChance = this.dropChance,
            inventoryItem = this.inventoryItem.Clone()
        };
    }
}
