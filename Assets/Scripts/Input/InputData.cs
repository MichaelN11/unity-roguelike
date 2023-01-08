using System;
using UnityEngine;

/// <summary>
/// POCO for storing input for an entity.
/// </summary>
public class InputData
{
    public InputType Type { get; set; }
    public Vector2 Direction { get; set; }
}
