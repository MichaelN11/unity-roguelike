using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component for an openable chest object.
/// </summary>
public class Chest : MonoBehaviour, IInteractable
{
    public bool Interact(InteractableUser interactableUser)
    {
        Debug.Log("Chest opened!");
        return true;
    }
}
