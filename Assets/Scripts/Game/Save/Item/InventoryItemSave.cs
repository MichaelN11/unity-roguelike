using System;

/// <summary>
/// POCO representing an item in an inventory.
/// </summary>
[Serializable]
public class InventoryItemSave
{
    public string Name { get; set; }
    public int Amount { get; set; }
    public string LearnableAbilityName { get; set; }
}
