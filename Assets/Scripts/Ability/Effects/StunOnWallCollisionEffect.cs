using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Effect that makes an entity get stunned when it collides with a wall.
/// </summary>
[CreateAssetMenu(menuName = "Game Data/Ability Effect/Stun on Wall Collision")]
public class StunOnWallCollisionEffect : AbilityEffect
{
    [SerializeField]
    private float stunDuration;
    public float StunDuration => stunDuration;

    [SerializeField]
    private float knockbackSpeed = 0;
    public float KnockbackSpeed => knockbackSpeed;

    [SerializeField]
    private float knockbackAcceleration = 0;
    public float KnockbackAcceleration => knockbackAcceleration;

    [SerializeField]
    private Sound wallCollisionSound;
    public Sound WallCollisionSound => wallCollisionSound;

    public override void Trigger(AbilityUseData abilityUseData, EffectUseData effectUseData)
    {
        if (abilityUseData.Movement != null)
        {
            abilityUseData.Movement.OnWallCollision += CollideWithWall;
        }
    }

    public override void Unapply(AbilityUseData abilityUseData, EffectUseData effectUseData)
    {
        if (abilityUseData.Movement != null)
        {
            abilityUseData.Movement.OnWallCollision -= CollideWithWall;
        }
    }

    private void CollideWithWall(WallCollisionEvent collisionEvent)
    {
        AudioManager.Instance.Play(wallCollisionSound);
        if (stunDuration > 0)
        {
            collisionEvent.EntityState.HitstunState(stunDuration);
            collisionEvent.Movement.SetMovement(collisionEvent.Direction * -1,
                knockbackSpeed, knockbackAcceleration);
        }
    }
}
