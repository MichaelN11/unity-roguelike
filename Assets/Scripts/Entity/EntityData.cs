using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// POCO storing an entity's state.
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
    /// Returns if the entity is flashing.
    /// </summary>
    /// <returns>true if the entity is flashing</returns>
    public bool IsFlashing()
    {
        return FlashTimer > 0;
    }

    /// <summary>
    /// Returns if the entity is stopped.
    /// </summary>
    /// <returns>true if the entity is stopped</returns>
    public bool IsStopped()
    {
        return StopTimer > 0;
    }
}
