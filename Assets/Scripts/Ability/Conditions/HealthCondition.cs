using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Condition that checks if an entity's health is within a specified range.
/// </summary>
[CreateAssetMenu(menuName = "Game Data/Condition/Health")]
public class HealthCondition : AbilityCondition
{
    [SerializeField]
    private float minimumHealthPercent = 0;
    [SerializeField]
    private float maximumHealthPercent = 100;

    public override bool ConditionMet(AbilityUseData abilityUseData, EntityAbilityContext entityAbilityContext)
    {
        float currentHealthPercent = (abilityUseData.Damageable.CurrentHealth / abilityUseData.Damageable.MaxHealth) * 100;
        return (currentHealthPercent >= minimumHealthPercent && currentHealthPercent <= maximumHealthPercent);
    }
}
