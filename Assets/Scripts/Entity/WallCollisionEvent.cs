using System;
using UnityEngine;

/// <summary>
/// Contains data for a collision event with a wall.
/// </summary>
public class WallCollisionEvent
{
    public Vector2 Direction { get; set; }
    public Movement Movement { get; set; }
    public EntityState EntityState { get; set; }
}
