using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Component for managing an ability icon on the UI.
/// </summary>
public class AbilityIcon : MonoBehaviour
{
    [SerializeField]
    private Image cooldownImage;

    [SerializeField]
    private int abilityNumber;

    private AbilityManager playerAbilityManager;
    private CanvasGroup canvasGroup;
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    private void Update()
    {
        if (playerAbilityManager != null)
        {
            UpdateFromAbility();
        } else if (PlayerController.Instance != null)
        {
            playerAbilityManager = PlayerController.Instance.GetComponentInChildren<AbilityManager>();
            if (abilityNumber < playerAbilityManager.Abilities.Count)
            {
                Show();
            }
        }
    }

    public void Show()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        if (abilityNumber < playerAbilityManager.Abilities.Count)
        {
            ActiveAbilityContext ability = playerAbilityManager.Abilities[abilityNumber];
            if (image.sprite != ability.Ability.AbilityIcon)
            {
                image.sprite = ability.Ability.AbilityIcon;
            }

        }
    }

    private void UpdateFromAbility()
    {
        if (abilityNumber < playerAbilityManager.Abilities.Count)
        {
            ActiveAbilityContext ability = playerAbilityManager.Abilities[abilityNumber];
            if (ability.Ability.Cooldown > 0)
            {
                cooldownImage.fillAmount = ability.CurrentCooldown / ability.Ability.Cooldown;
            }
        } else
        {
            Hide();
        }
    }

    private void Hide()
    {
        canvasGroup.alpha = 0f;          // fully transparent
        canvasGroup.interactable = false; // stops clicks
        canvasGroup.blocksRaycasts = false; // stops blocking other UI clicks
    }
}
