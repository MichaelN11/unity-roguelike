using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ability effect that heals the entity.
/// </summary>
[CreateAssetMenu(menuName = "Game Data/Ability Effect/Heal")]
public class HealEffect : AbilityEffect
{
    [SerializeField]
    private float healAmount;
    public float HealAmount => healAmount;

    public override void Trigger(EffectData effectData)
    {
        effectData.Damageable.Heal(healAmount);
    }
}
