using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component for an openable chest object.
/// </summary>
public class Chest : MonoBehaviour, IInteractable
{
    private const float FloatingTextXOffset = -0.1f;

    [SerializeField]
    private GameObject itemFloatingText;
    [SerializeField]
    private Sound sound;
    [SerializeField]
    private float interactionTime = 1f;

    private bool opened = false;
    public bool Opened => opened;

    private Animator animator;
    private LevelObject levelObject;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        levelObject = GetComponent<LevelObject>();
    }

    private void Start()
    {
        if (levelObject == null || levelObject.containedItem.Item == null || levelObject.containedItem.Amount <= 0)
        {
            SetToOpen();
        }
    }

    public bool Interact(InteractableUser interactableUser)
    {
        if (!IsAbleToInteract(interactableUser))
        {
            return false;
        }

        interactableUser.Movement.StopMoving();
        interactableUser.EntityState.InteractState(interactionTime);
        AudioManager.Instance.Play(sound);

        if (animator != null)
        {
            animator.SetTrigger("open");
        }

        if (levelObject != null && levelObject.containedItem.Item != null && levelObject.containedItem.Amount > 0)
        {
            interactableUser.Inventory.AcquireItem(levelObject.containedItem);
            Vector2 position = new(transform.position.x + FloatingTextXOffset, transform.position.y);
            GameObject floatingText = Instantiate(itemFloatingText, position, Quaternion.identity);
            floatingText.GetComponent<ItemFloatingText>().Init(levelObject.containedItem);
            levelObject.containedItem.Item = null;
            levelObject.containedItem.Amount = 0;
        }

        opened = true;

        return true;
    }

    public bool IsAbleToInteract(InteractableUser interactableUser)
    {
        return !opened;
    }

    private void SetToOpen()
    {
        if (animator != null)
        {
            animator.SetTrigger("openImmediate");
        }
        opened = true;
    }
}
