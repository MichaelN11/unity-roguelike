using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// POCO for an item in an inventory.
/// </summary>
[Serializable]
public class InventoryItem
{
    public Item Item { get; set; }
    public int Amount { get; set; }
}
