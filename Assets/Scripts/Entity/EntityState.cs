using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// POCO storing an entity's state.
/// </summary>
public class EntityState
{
    public Vector2 LookDirection { get; set; } = Vector2.zero;
    public Action Action { get; set; } = Action.Stand;
    public float StunTimer { get; set; } = 0f;
}
