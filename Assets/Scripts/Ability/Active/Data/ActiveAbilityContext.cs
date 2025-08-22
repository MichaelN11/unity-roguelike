using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Active ability wrapper class for an ability belonging to a specific entity.
/// </summary>
public class ActiveAbilityContext
{
    public ActiveAbility Ability { get; set; }
    public float CurrentCooldown { get; set; }
}
