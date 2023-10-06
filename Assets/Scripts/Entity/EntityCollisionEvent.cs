using System;
using UnityEngine;

/// <summary>
/// Contains data for a collision event between two entities.
/// </summary>
public class EntityCollisionEvent
{
    public Rigidbody2D SourceBody { get; set; }
    public Rigidbody2D TargetBody { get; set; }

    public EntityState SourceEntityState { get; set; }
    public EntityData SourceEntityData { get; set; }
}
