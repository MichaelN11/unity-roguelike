using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Utility class for selecting an item to drop.
/// </summary>
public class ItemDropUtil : MonoBehaviour
{
    public static ItemDrop GetRandomItemDrop(DropTable dropTable, bool removeFromTable = false)
    {
        if (dropTable.IsWeighted)
        {
            return GetRandomItemDropWeighted(dropTable.ItemDrops, removeFromTable);
        } else
        {
            return GetRandomItemDrop(dropTable.ItemDrops, 0f, removeFromTable);
        }
    }

    public static ItemDrop GetRandomItemDrop(List<ItemDrop> itemDrops, float dropChanceModifier = 0f, bool removeFromTable = false)
    {
        ItemDrop randomItemDrop = null;
        float multiplier = 1 - dropChanceModifier;
        float randomValue = Random.value * multiplier;

        float nextDropChance = 0;
        int index = 0;
        foreach (ItemDrop itemDrop in itemDrops)
        {
            nextDropChance += Mathf.Max(0, itemDrop.DropChance);
            if (nextDropChance > 1)
            {
                Debug.LogWarning("Error: Drop table is greater than 1.");
            }
            if (randomValue <= nextDropChance)
            {
                randomItemDrop = itemDrop.Clone();
                break;
            }
            ++index;
        }

        if (removeFromTable && index < itemDrops.Count)
        {
            itemDrops.RemoveAt(index);
        }

        return randomItemDrop;
    }

    public static ItemDrop GetRandomItemDropWeighted(List<ItemDrop> itemDrops, bool removeFromTable = false)
    {
        ItemDrop randomItemDrop = null;
        float totalWeight = 0;
        foreach (ItemDrop itemDrop in itemDrops)
        {
            totalWeight += itemDrop.DropChance;
        }

        float randomValue = Random.value;
        float nextDropChance = 0;
        int index = 0;
        foreach (ItemDrop itemDrop in itemDrops)
        {
            nextDropChance += itemDrop.DropChance / totalWeight;
            if (randomValue <= nextDropChance)
            {
                randomItemDrop = itemDrop.Clone();
                break;
            }
            ++index;
        }

        if (removeFromTable && index < itemDrops.Count)
        {
            itemDrops.RemoveAt(index);
        }

        return randomItemDrop;
    }
}
