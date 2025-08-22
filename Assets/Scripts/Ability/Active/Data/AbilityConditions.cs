using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that stores conditions required to use an ability.
/// </summary>
[Serializable]
public class AbilityConditions
{
    [SerializeField]
    private float minimumHealthPercent = 0;
    [SerializeField]
    private float maximumHealthPercent = 100;

    public bool ConditionsMet(AbilityUseData abilityUseData)
    {
        float currentHealthPercent = (abilityUseData.Damageable.CurrentHealth / abilityUseData.Damageable.MaxHealth) * 100;
        return (currentHealthPercent >= minimumHealthPercent && currentHealthPercent <= maximumHealthPercent);
    }
}
