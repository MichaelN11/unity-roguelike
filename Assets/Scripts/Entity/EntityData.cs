using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// POCO storing an entity's state.
/// </summary>
public class EntityData
{
    public Vector2 LookDirection { get; set; } = Vector2.zero;
    public ActionState ActionState { get; set; } = ActionState.Stand;
    public float StunTimer { get; set; } = 0f;
    public float FlashTimer { get; set; } = 0f;
    public AttackAnimation AttackAnimation { get; set; } = AttackAnimation.Default;
}
