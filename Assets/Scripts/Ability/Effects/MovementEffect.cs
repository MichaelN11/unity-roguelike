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
    private float accelerationDelay = 0;
    public float AccelerationDelay => accelerationDelay;

    private IEnumerator coroutine;

    public override void Trigger(EffectData effectData)
    {
        if (effectData.EntityMovement != null)
        {
            if (accelerationDelay <= 0)
            {
                effectData.EntityMovement.SetMovement(effectData.Direction,
                    moveSpeed,
                    moveAcceleration);
                
            } else
            {
                effectData.EntityMovement.SetMovement(effectData.Direction,
                    moveSpeed);
                coroutine = DelayMovement(effectData);
                effectData.EntityMovement.StartCoroutine(coroutine);
            }
        }
    }

    private IEnumerator DelayMovement(EffectData effectData)
    {
        yield return new WaitForSeconds(accelerationDelay);
        effectData.EntityMovement.SetMovement(effectData.Direction,
                    moveSpeed,
                    moveAcceleration);
    }
}
