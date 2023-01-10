using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class representing a grid used for pathfinding.
/// </summary>
public class PathingGrid
{
    public const float AdjacentMoveCost = 10;
    public const float DiagonalMoveCost = 14;

    public List<List<GridNode>> Grid { get; set; } = new();
    public GridLayout GridLayout { get; set; }
}
