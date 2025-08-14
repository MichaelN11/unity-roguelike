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
    private GameObject abilityFloatingText;
    [SerializeField]
    private Sound sound;
    [SerializeField]
    private Sound newAbilitySound;
    [SerializeField]
    private float interactionTime = 1f;
    [SerializeField]
    private float itemSoundDelay = 0;
    [SerializeField]
    private float abilitySoundDelay = 0.5f;

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
        if (IsEmpty(levelObject))
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

        if (levelObject != null)
        {
            if (levelObject.ContainedItem.Item != null && levelObject.ContainedItem.Amount > 0)
            {
                if (levelObject.ContainedItem.Item.PickupSound)
                {
                    IEnumerator delayedSoundCoroutine = DelayedSoundCoroutine(levelObject.ContainedItem.Item.PickupSound, itemSoundDelay);
                    StartCoroutine(delayedSoundCoroutine);
                }
                Vector2 position = new(transform.position.x + FloatingTextXOffset, transform.position.y);
                GameObject floatingText = Instantiate(itemFloatingText, position, Quaternion.identity);
                floatingText.GetComponent<ItemFloatingText>().Init(levelObject.ContainedItem);

                interactableUser.Inventory.AcquireItem(levelObject.ContainedItem);
                levelObject.ContainedItem.Item = null;
                levelObject.ContainedItem.Amount = 0;
            } else if (levelObject.ContainedItem.LearnableAbility)
            {
                Vector2 position = new(transform.position.x + FloatingTextXOffset, transform.position.y);
                GameObject floatingText = Instantiate(abilityFloatingText, position, Quaternion.identity);
                string displayText = "New ability: " + levelObject.ContainedItem.LearnableAbility.AbilityName;
                floatingText.GetComponent<ItemFloatingText>().Init(displayText, 0);

                int abilityNumber = interactableUser.AbilityManager.LearnNewAbility(levelObject.ContainedItem.LearnableAbility);
                if (abilityNumber >= 0)
                {
                    IEnumerator delayedSoundCoroutine = DelayedSoundCoroutine(newAbilitySound, abilitySoundDelay);
                    StartCoroutine(delayedSoundCoroutine);

                    UIController.Instance.AbilityIconAnimator.StartMovingIconAnimation(transform.position, abilityNumber,
                        levelObject.ContainedItem.LearnableAbility.AbilityIcon);
                }
                levelObject.ContainedItem.LearnableAbility = null;
            }
        }

        opened = true;

        return true;
    }

    public bool IsAbleToInteract(InteractableUser interactableUser)
    {
        return !opened;
    }

    private bool IsEmpty(LevelObject levelObject)
    {
        return levelObject == null
            || (levelObject.ContainedItem.Item == null && levelObject.ContainedItem.LearnableAbility == null);
    }

    private void SetToOpen()
    {
        if (animator != null)
        {
            animator.SetTrigger("openImmediate");
        }
        opened = true;
    }

    private IEnumerator DelayedSoundCoroutine(Sound sound, float delay)
    {
        yield return new WaitForSeconds(delay);
        AudioManager.Instance.Play(sound);
    }
}
