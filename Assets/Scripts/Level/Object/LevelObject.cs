using UnityEngine;
using System.Collections;

/// <summary>
/// Component indicating an object in the level that should be saved. Can contain an item.
/// </summary>
public class LevelObject : MonoBehaviour
{
    [field: SerializeField]
    public string Type { get; set; }
    [field: SerializeField]
    public InventoryItem ContainedItem { get; set; } = new();
    [field: SerializeField]
    public DropTable DropTable { get; set; }

    private void Start()
    {
        if (ContainedItem.Item == null && ContainedItem.LearnableAbility == null && DropTable != null)
        {
            ItemDrop itemDrop = ItemDropUtil.GetRandomItemDrop(DropTable);
            if (itemDrop != null)
            {
                ContainedItem = itemDrop.InventoryItem;
            }
        }
    }
}
