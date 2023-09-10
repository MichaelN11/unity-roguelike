using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class storing data for an effect being triggered.
/// </summary>
public class EffectData
{
    public GameObject Entity { get; set; }
    public EntityData EntityData { get; set; }
    public EntityState EntityState { get; set; }
    public Movement EntityMovement { get; set; }

    public Vector2 Position { get; set; }
    public Vector2 Direction { get; set; } = Vector2.zero;
}
