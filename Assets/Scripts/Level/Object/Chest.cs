using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component for an openable chest object.
/// </summary>
public class Chest : MonoBehaviour, IInteractable
{
    private const float FloatingTextXOffset = -0.1f;

    public InventoryItem containedItem = new();

    [SerializeField]
    private GameObject itemFloatingText;
    [SerializeField]
    private Sound sound;

    private Animator animator;
    private bool opened = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public bool Interact(InteractableUser interactableUser)
    {
        if (opened)
        {
            return false;
        }

        AudioManager.Instance.Play(sound);

        if (animator != null)
        {
            animator.SetTrigger("open");
        }

        if (containedItem.Item != null && containedItem.Amount > 0)
        {
            interactableUser.Inventory.AddItem(containedItem);
            Vector2 position = new(transform.position.x + FloatingTextXOffset, transform.position.y);
            GameObject floatingText = Instantiate(itemFloatingText, position, Quaternion.identity);
            floatingText.GetComponent<ItemFloatingText>().Init(containedItem);
        }

        opened = true;

        return true;
    }
}
