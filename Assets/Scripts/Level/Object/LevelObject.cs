using UnityEngine;
using System.Collections;

public class LevelObject : MonoBehaviour
{
    public string type { get; set; }
    public InventoryItem containedItem = new();
}
