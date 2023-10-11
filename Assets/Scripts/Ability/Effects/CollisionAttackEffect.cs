using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Ability effect that causes the entity to deal attack damage when it collides with other entities.
/// </summary>
[CreateAssetMenu(menuName = "Game Data/Ability Effect/Collision Attack")]
public class CollisionAttackEffect : AbilityEffect
{
    [SerializeField]
    private AttackEffectData attackEffectData;
    public AttackEffectData AttackEffectData => attackEffectData;

    public override void Trigger(EffectData effectData)
    {
        if (effectData.Hitbox != null)
        {
            effectData.Hitbox.OnEntityCollision += AttackOnCollision;
            effectData.Hitbox.ResetHitTimer();
        }
    }

    public override void Unapply(EffectData effectData)
    {
        if (effectData.Hitbox != null)
        {
            effectData.Hitbox.OnEntityCollision -= AttackOnCollision;
        }
    }

    private void AttackOnCollision(EntityCollisionEvent entityCollisionEvent)
    {
        AttackData attackData = new();
        attackData.AttackEffectData = attackEffectData;
        attackData.User = entityCollisionEvent.SourceBody.gameObject;
        attackData.UserEntityData = entityCollisionEvent.SourceEntityData;
        attackData.UserEntityState = entityCollisionEvent.SourceEntityState;
        AttackHandler.AttackEntity(attackData, entityCollisionEvent.SourceBody, entityCollisionEvent.TargetBody);
    }
}
