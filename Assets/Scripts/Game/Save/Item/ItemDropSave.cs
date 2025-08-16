using System;

/// <summary>
/// Class containing item/ability drop save data.
/// </summary>
[Serializable]
public class ItemDropSave
{
    public float DropChance { get; set; }
    public InventoryItemSave InventoryItemSave { get; set; }
}
