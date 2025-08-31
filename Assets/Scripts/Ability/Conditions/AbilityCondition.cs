using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract class marking a condition required to use an ability.
/// </summary>
public abstract class AbilityCondition : ScriptableObject
{
    public abstract bool ConditionMet(AbilityUseData abilityUseData, EntityAbilityContext entityAbilityContext);
}
