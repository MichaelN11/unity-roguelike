using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Component for managing a UI menu with a save button. The button is disabled if the player is in combat.
/// </summary>
public class MenuWithSaveButton : MonoBehaviour
{
    [SerializeField]
    private Button saveButton;

    private void OnEnable()
    {
        UpdateButtonState();
    }

    private void UpdateButtonState()
    {
        if (GameManager.Instance != null)
        {
            bool playerInCombat = GameManager.Instance.IsPlayerInCombat();
            if (saveButton.IsInteractable() && playerInCombat)
            {
                saveButton.interactable = false;
            }
            else if (!saveButton.IsInteractable() && !playerInCombat)
            {
                saveButton.interactable = true;
            }
        }
    }
}
