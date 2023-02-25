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
    public AttackAnimation AttackAnimation { get; set; } = AttackAnimation.Default;

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
