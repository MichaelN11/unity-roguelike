using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Utility class for selecting an item to drop.
/// </summary>
public class ItemDropUtil : MonoBehaviour
{
    public static ItemDrop GetRandomItemDrop(DropTable dropTable)
    {
        if (dropTable.IsWeighted)
        {
            return GetRandomItemDropWeighted(dropTable.ItemDrops);
        } else
        {
            return GetRandomItemDrop(dropTable.ItemDrops);
        }
    }

    public static ItemDrop GetRandomItemDrop(List<ItemDrop> itemDrops, float dropChanceModifier = 0f)
    {
        float multiplier = 1 - dropChanceModifier;
        float randomValue = Random.value * multiplier;

        float nextDropChance = 0;
        foreach (ItemDrop itemDrop in itemDrops)
        {
            nextDropChance += Mathf.Max(0, itemDrop.DropChance);
            if (nextDropChance > 1)
            {
                Debug.Log("Error: Drop table is greater than 1.");
            }
            if (randomValue <= nextDropChance)
            {
                return itemDrop.Clone();
            }
        }

        return null;
    }

    public static ItemDrop GetRandomItemDropWeighted(List<ItemDrop> itemDrops)
    {
        float totalWeight = 0;
        foreach (ItemDrop itemDrop in itemDrops)
        {
            totalWeight += itemDrop.DropChance;
        }

        float randomValue = Random.value;
        float nextDropChance = 0;
        foreach (ItemDrop itemDrop in itemDrops)
        {
            nextDropChance += itemDrop.DropChance / totalWeight;
            if (randomValue <= nextDropChance)
            {
                return itemDrop.Clone();
            }
        }

        return null;
    }
}
