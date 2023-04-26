using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// POCO representing a saved tile in a level.
/// </summary>
[Serializable]
public class TileSave
{
    public string Name { get; set; }
    public Vector2 Position { get; set; }
    public bool IsFlipped { get; set; }
}
