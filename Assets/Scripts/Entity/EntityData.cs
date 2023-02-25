using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class storing an entity's state.
/// </summary>
public class EntityData
{
    public Vector2 LookDirection { get; set; } = Vector2.zero;
    public AttackAnimation AttackAnimation { get; set; } = AttackAnimation.Default;
}
