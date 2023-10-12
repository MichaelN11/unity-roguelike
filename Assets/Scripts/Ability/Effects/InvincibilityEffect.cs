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

    public override void Trigger(EffectData effectData)
    {
        effectData.Damageable.SetInvincibility(Duration);
        if (passThroughEnemies)
        {
            effectData.Movement.PassThroughEntities(Duration);
        }
    }

    public override void Unapply(EffectData effectData)
    {
        effectData.Damageable.SetInvincibility(0);
        if (passThroughEnemies)
        {
            effectData.Movement.StopPassingThroughEntities();
        }
    }
}
