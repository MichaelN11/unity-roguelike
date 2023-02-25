using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract scriptable object representing an ability.
/// </summary>
public abstract class Ability : ScriptableObject
{
    public abstract AbilityBehavior BuildBehavior(AbilityManager abilityManager);
}
