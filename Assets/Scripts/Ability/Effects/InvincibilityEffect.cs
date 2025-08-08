using System;
using UnityEngine;

/// <summary>
/// An AbilityEffect that causes the entity to become invincible.
/// </summary>
[CreateAssetMenu(menuName = "Game Data/Ability Effect/Invincibility")]
public class InvincibilityEffect : AbilityEffect
{
    [SerializeField]
    private bool passThroughEnemies;
    public bool PassThroughEnemies => passThroughEnemies;

    public override void Trigger(AbilityUseData abilityUseData, EffectUseData effectUseData)
    {
        abilityUseData.Damageable.SetInvincibility(Duration);
        if (passThroughEnemies)
        {
            abilityUseData.Movement.PassThroughEntities(Duration);
        }
    }

    public override void Unapply(AbilityUseData abilityUseData, EffectUseData effectUseData)
    {
        abilityUseData.Damageable.SetInvincibility(0);
        if (passThroughEnemies)
        {
            abilityUseData.Movement.StopPassingThroughEntities();
        }
    }
}
