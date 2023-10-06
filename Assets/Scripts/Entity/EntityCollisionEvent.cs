using System;
using UnityEngine;

/// <summary>
/// Contains data for a collision event between two entities.
/// </summary>
public class EntityCollisionEvent
{
    public GameObject SourceObject { get; set; }
    public Collider2D TargetCollider { get; set; }
}
