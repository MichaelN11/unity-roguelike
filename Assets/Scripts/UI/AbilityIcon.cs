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

    private void Update()
    {
        if (playerAbilityManager != null)
        {
            UpdateCooldownImage();
        } else if (PlayerController.Instance != null)
        {
            playerAbilityManager = PlayerController.Instance.GetComponentInChildren<AbilityManager>();
        }
    }

    private void UpdateCooldownImage()
    {
        ActiveAbilityContext ability = playerAbilityManager.Abilities[abilityNumber];
        if (ability.Ability.Cooldown > 0)
        {
            cooldownImage.fillAmount = ability.CurrentCooldown / ability.Ability.Cooldown;
        }
    }
}
