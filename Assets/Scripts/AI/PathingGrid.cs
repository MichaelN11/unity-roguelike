using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Class representing a grid used for pathfinding.
/// </summary>
public class PathingGrid
{
    public const float StraightMoveCost = 10;
    public const float DiagonalMoveCost = 14;

    public List<List<GridNode>> Grid { get; set; } = new();
    public Tilemap Tilemap { private get; set; }

    /// <summary>
    /// Converts the passed world position to a GridNode in the grid.
    /// </summary>
    /// <param name="worldPosition">The world position as a Vector2</param>
    /// <returns>The corresponding GridNode</returns>
    public GridNode WorldToNode(Vector2 worldPosition)
    {
        Vector3Int tilemapCell = Tilemap.WorldToCell(worldPosition);
        Vector3Int gridCell = TilemapCellToGridCell(tilemapCell);
        return Grid[gridCell.x][gridCell.y];
    }

    /// <summary>
    /// Converts the passed GridNode to a position in world space, using the center
    /// of the node.
    /// </summary>
    /// <param name="node">The GridNode</param>
    /// <returns>The corresponding world position as a Vector2</returns>
    public Vector2 NodeToWorld(GridNode node)
    {
        Vector3Int gridCell = new(node.X, node.Y, 0);
        Vector3Int tilemapCell = GridCellToTilemapCell(gridCell);
        return Tilemap.GetCellCenterWorld(tilemapCell);
    }

    /// <summary>
    /// Converts a cell in the grid to a cell in the tilemap. The grid starts at (0,0)
    /// and only uses positive integer positions.
    /// </summary>
    /// <param name="gridCell">The Vector3Int cell position in the grid</param>
    /// <returns>The Vector3Int cell position in the tilemap</returns>
    private Vector3Int GridCellToTilemapCell(Vector3Int gridCell)
    {
        BoundsInt cellBounds = Tilemap.cellBounds;
        return gridCell + cellBounds.min;
    }

    /// <summary>
    /// Converts a cell in the tilemap to a cell in the grid. The grid starts at (0,0)
    /// and only uses positive integer positions.
    /// </summary>
    /// <param name="tilemapCell">The Vector3Int cell position in the tilemap</param>
    /// <returns>The Vector3Int cell position in the grid</returns>
    private Vector3Int TilemapCellToGridCell(Vector3Int tilemapCell)
    {
        BoundsInt cellBounds = Tilemap.cellBounds;
        return tilemapCell - cellBounds.min;
    }
}
