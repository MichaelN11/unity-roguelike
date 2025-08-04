using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Component for managing a UI menu with a load button. The button is disabled if saved data does not exist.
/// </summary>
public class MenuWithLoadButton : MonoBehaviour
{
    [SerializeField]
    private Button loadButton;

    private void OnEnable()
    {
        UpdateButtonState();
    }

    private void Start()
    {
        UpdateButtonState();
    }

    private void UpdateButtonState()
    {
        if (GameManager.Instance != null)
        {
            bool savedDataExists = GameManager.Instance.SavedDataExists();
            if (loadButton.IsInteractable() && !savedDataExists)
            {
                loadButton.interactable = false;
            }
            else if (!loadButton.IsInteractable() && savedDataExists)
            {
                loadButton.interactable = true;
            }
        }
    }
}
