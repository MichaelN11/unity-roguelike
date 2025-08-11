using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class containing data about an entity using an interactable.
/// </summary>
public class InteractableUser
{
    public Inventory Inventory { get; set; }
    public EntityState EntityState { get; set; }
    public Movement Movement { get; set; }
    public AbilityManager AbilityManager { get; set; }
}
