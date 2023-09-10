using System;
using UnityEngine;

/// <summary>
/// An AbilityEffect that causes the entity to move.
/// </summary>
[CreateAssetMenu(menuName = "Game Data/Ability Effect/Movement")]
public class MovementAttackEffect : AbilityEffect
{
    [SerializeField]
    private float moveSpeed = 0;
    public float MoveSpeed => moveSpeed;

    [SerializeField]
    private float moveAcceleration = 0;
    public float MoveAcceleration => moveAcceleration;

    public override void Trigger(EffectData effectData)
    {
        if (effectData.EntityMovement != null)
        {
            effectData.EntityMovement.SetMovement(effectData.Direction,
                moveSpeed,
                moveAcceleration);
        }
    }
}
