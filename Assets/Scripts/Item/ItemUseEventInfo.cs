using System;

/// <summary>
/// POCO storing data about an item use event.
/// </summary>
public class ItemUseEventInfo
{
    public Item Item { get; set; }
    public AbilityUseEventInfo AbilityUseEventInfo { get; set; }
}
