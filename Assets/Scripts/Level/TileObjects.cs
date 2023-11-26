using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class representing the objects built from a tile.
/// </summary>
public class TileObjects
{
    public Vector2 TileCenter { get; set; }
    public List<GameObject> ObjectList { get; set; } = new();
}
