using UnityEngine;
using System.Collections;

/// <summary>
/// Component indicating an object in the level that should be saved. Can contain an item.
/// </summary>
public class LevelObject : MonoBehaviour
{
    public string Type { get; set; }
    public InventoryItem containedItem = new();
}
