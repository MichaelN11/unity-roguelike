using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ability effect that increases an entity's maximum health.
/// </summary>
[CreateAssetMenu(menuName = "Game Data/Ability Effect/Max Health Increase")]
public class MaxHealthIncreaseEffect : AbilityEffect
{
    [SerializeField]
    private float maxHealthIncreaseAmount;
    public float MaxHealthIncreaseAmount => maxHealthIncreaseAmount;

    public override void Trigger(AbilityUseData abilityUseData, EffectUseData effectUseData)
    {
        abilityUseData.Damageable.IncreaseMaxHealth(maxHealthIncreaseAmount);
    }
}
