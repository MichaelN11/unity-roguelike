using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// POCO storing data passed into an ability when it is used.
/// </summary>
public class AbilityUse
{
    public EntityType EntityType { get; set; }
    public AbilityManager Component { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Direction { get; set; } = Vector2.zero;
}
