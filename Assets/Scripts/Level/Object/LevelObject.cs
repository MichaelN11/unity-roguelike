using UnityEngine;
using System.Collections;

/// <summary>
/// Component indicating an object in the level that should be saved. Can contain an item.
/// </summary>
public class LevelObject : MonoBehaviour
{
    public string Type { get; set; }
    [field: SerializeField]
    public InventoryItem ContainedItem { get; set; } = new();
    [field: SerializeField]
    public ActiveAbility Ability { get; set; } = null;
}
