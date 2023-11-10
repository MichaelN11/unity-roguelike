using System;

/// <summary>
/// POCO representing an item in an inventory.
/// </summary>
[Serializable]
public class InventoryItemSave
{
    public String Name { get; set; }
    public int Amount { get; set; }
}
