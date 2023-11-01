using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Component for managing an item displaying in the UI.
/// </summary>
public class ItemIcon : MonoBehaviour
{
    [SerializeField]
    private int itemNumber;
    [SerializeField]
    private Image icon;
    [SerializeField]
    private TextMeshProUGUI quantity;

    private bool foundPlayer = false;
    private InventoryItem inventoryItem;

    private void Update()
    {
        if (!foundPlayer && PlayerController.Instance != null)
        {
            foundPlayer = true;
            if (PlayerController.Instance.TryGetComponent<Inventory>(out Inventory playerInventory))
            {
                if (itemNumber <= playerInventory.Items.Count)
                {
                    inventoryItem = playerInventory.Items[itemNumber - 1];
                    if (inventoryItem.Item.Icon != null)
                    {
                        icon.sprite = inventoryItem.Item.Icon;
                    }
                }
            }
            if (inventoryItem == null)
            {
                gameObject.SetActive(false);
            }
        }
        if (inventoryItem != null)
        {
            quantity.text = inventoryItem.Amount.ToString();
        }
    }
}
