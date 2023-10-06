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

    [SerializeField]
    private float duration;
    public float Duration => duration;

    private IEnumerator coroutine;

    public override void Trigger(EffectData effectData)
    {
        effectData.Hitbox.OnEntityCollision += AttackOnCollision;
        effectData.Hitbox.ResetHitTimer();

        coroutine = UnsubscribeFromHitbox(effectData.Hitbox);
        effectData.AbilityManager.StartCoroutine(coroutine);
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

    private IEnumerator UnsubscribeFromHitbox(Hitbox hitbox)
    {
        yield return new WaitForSeconds(duration);
        hitbox.OnEntityCollision -= AttackOnCollision;
    }
}
