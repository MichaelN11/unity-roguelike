using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class storing an entity's state.
/// </summary>
public class EntityData
{
    public Vector2 MoveDirection { get; set; } = Vector2.zero;
    public float MoveSpeed { get; set; } = 0;
    public float Acceleration { get; set; } = 0;
    public bool MovementStopped { get; set; } = false;
    public Vector2 LookDirection { get; set; } = Vector2.zero;
    public ActionState ActionState { get; set; } = ActionState.Stand;
    public float StunTimer { get; set; } = 0f;
    public float FlashTimer { get; set; } = 0f;
    public float StopTimer { get; set; } = 0f;
    public AttackAnimation AttackAnimation { get; set; } = AttackAnimation.Default;

    /// <summary>
    /// Determines if the entity is flashing.
    /// </summary>
    /// <returns>true if the entity is flashing</returns>
    public bool IsFlashing()
    {
        return FlashTimer > 0;
    }

    /// <summary>
    /// Determines if the entity is stopped.
    /// </summary>
    /// <returns>true if the entity is stopped</returns>
    public bool IsStopped()
    {
        return StopTimer > 0;
    }

    /// <summary>
    /// Determines if the entity is stunned.
    /// </summary>
    /// <returns>true if the entity is stunned</returns>
    public bool IsStunned()
    {
        return ActionState == ActionState.UsingAbility
            || ActionState == ActionState.Hitstun;
    }

    /// <summary>
    /// Determines if the entity is able to act.
    /// </summary>
    /// <returns>true if the entity can act</returns>
    public bool CanAct()
    {
        //Debug.Log("Checking if entity can act. Current state: " + ActionState);
        return !IsStunned()
            && ActionState != ActionState.Dead;
    }

    /// <summary>
    /// Changes state to the UsingAbility state, using the passed duration.
    /// </summary>
    /// <param name="duration">The time in the ability state as a float</param>
    public void ChangeToAbilityState(float duration)
    {
        ActionState = ActionState.UsingAbility;
        StunTimer = duration;
    }

    /// <summary>
    /// Stops the entity from moving.
    /// </summary>
    public void StopMoving()
    {
        MoveDirection = Vector2.zero;
        MoveSpeed = 0;
        Acceleration = 0;
    }
}
