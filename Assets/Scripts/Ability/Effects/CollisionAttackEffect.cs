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

    public override void Trigger(AbilityUseData abilityUseData, EffectUseData effectUseData)
    {
        if (abilityUseData.Hitbox != null)
        {
            abilityUseData.Hitbox.OnEntityCollision += AttackOnCollision;
            abilityUseData.Hitbox.ResetHitTimer();
        }
    }

    public override void Unapply(AbilityUseData abilityUseData, EffectUseData effectUseData)
    {
        if (abilityUseData.Hitbox != null)
        {
            abilityUseData.Hitbox.OnEntityCollision -= AttackOnCollision;
        }
    }

    private void AttackOnCollision(EntityCollisionEvent entityCollisionEvent)
    {
        AttackData attackData = new();
        attackData.User = entityCollisionEvent.SourceBody.gameObject;
        attackData.UserEntityData = entityCollisionEvent.SourceEntityData;
        attackData.UserEntityState = entityCollisionEvent.SourceEntityState;

        attackData.Damage = attackEffectData.Damage;
        attackData.HitStop = attackEffectData.HitStop;
        attackData.HitStunMultiplier = attackEffectData.HitStunMultiplier;
        attackData.KnockbackMultiplier = attackEffectData.KnockbackMultiplier;
        attackData.Description = attackEffectData.Description;

        AttackHandler.AttackEntity(attackData, entityCollisionEvent.SourceBody, entityCollisionEvent.TargetBody);
    }
}
