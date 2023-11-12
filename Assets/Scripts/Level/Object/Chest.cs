using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component for an openable chest object.
/// </summary>
public class Chest : MonoBehaviour, IInteractable
{
    public InventoryItem containedItem;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public bool Interact(InteractableUser interactableUser)
    {
        if (animator != null)
        {
            animator.SetTrigger("open");
        }

        if (containedItem.Item != null && containedItem.Amount > 0)
        {
            interactableUser.Inventory.AddItem(containedItem);
        }

        return true;
    }
}
