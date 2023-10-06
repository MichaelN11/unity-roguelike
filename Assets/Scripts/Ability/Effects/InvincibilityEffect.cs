using System;
using UnityEngine;

/// <summary>
/// An AbilityEffect that causes the entity to become invincible.
/// </summary>
[CreateAssetMenu(menuName = "Game Data/Ability Effect/Invincibility")]
public class InvincibilityEffect : AbilityEffect
{
    [SerializeField]
    private float duration;
    public float Duration => duration;

    [SerializeField]
    private bool passThroughEnemies;
    public bool PassThroughEnemies => passThroughEnemies;

    public override void Trigger(EffectData effectData)
    {
        effectData.Damageable.SetInvincibility(duration);
        if (passThroughEnemies)
        {
            effectData.Movement.PassThroughEntities(duration);
        }
    }
}
