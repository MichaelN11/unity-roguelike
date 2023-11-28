using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enum representing an action being taken by an entity.
/// </summary>
public enum ActionState
{
    None,
    Stand,
    Move,
    Ability,
    Hitstun,
    Idle,
    Dead,
    Interact
}
