using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Factory class for creating new inventory items.
/// </summary>
public class ItemFactory
{
    public static InventoryItem LoadItem(InventoryItemSave itemSave)
    {
        int itemAmount = itemSave.Amount;

        Item containedItem = null;
        ActiveAbility itemLearnableAbility = null;
        if (itemSave.LearnableAbilityName != null && itemSave.LearnableAbilityName != "")
        {
            itemLearnableAbility = GameManager.Instance.AddressableService.RetrieveAbility(itemSave.LearnableAbilityName);
        }
        if (itemSave.Name != null && itemSave.Name != "")
        {
            containedItem = GameManager.Instance.AddressableService.RetrieveItem(itemSave.Name);
        }

        return new()
        {
            Item = containedItem,
            Amount = itemAmount,
            LearnableAbility = itemLearnableAbility
        };
    }
}
