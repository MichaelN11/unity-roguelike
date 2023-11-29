using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for an interactable object. Should use the Interactable tag.
/// </summary>
public interface IInteractable
{
    public bool Interact(InteractableUser interactableUser);
    public bool IsAbleToInteract(InteractableUser interactableUser);
}
