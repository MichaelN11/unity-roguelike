using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// POCO representing an object in the level.
/// </summary>
[Serializable]
public class ObjectSave
{
    public string Type { get; set; }
    public Vector2 Position { get; set; }
    public InventoryItemSave InventoryItem { get; set; }
}
