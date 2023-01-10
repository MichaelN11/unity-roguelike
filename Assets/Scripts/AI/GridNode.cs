using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class representing a node in a grid used for pathfinding.
/// </summary>
public class GridNode
{
    public int X { get; set; }
    public int Y { get; set; }
    public bool Passable { get; set; }
    public List<GridAction> AdjacentActions { get; set; } = new();
}
