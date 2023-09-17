using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// An AbilityEffect that causes the entity to move.
/// </summary>
[CreateAssetMenu(menuName = "Game Data/Ability Effect/Movement")]
public class MovementEffect : AbilityEffect
{
    [SerializeField]
    private float moveSpeed = 0;
    public float MoveSpeed => moveSpeed;

    [SerializeField]
    private float moveAcceleration = 0;
    public float MoveAcceleration => moveAcceleration;

    [SerializeField]
    private float delayedAcceleration = 0;
    public float DelayedAcceleration => delayedAcceleration;

    [SerializeField]
    private float accelerationDelay = 0;
    public float AccelerationDelay => accelerationDelay;

    public override void Trigger(EffectData effectData)
    {
        if (effectData.EntityMovement != null)
        {
            effectData.EntityMovement.SetMovement(effectData.Direction,
                moveSpeed,
                moveAcceleration);
            
            if (accelerationDelay > 0)
            {
                effectData.EntityMovement.SetDelayedAcceleration(delayedAcceleration, accelerationDelay);
            }
        }
    }
}
