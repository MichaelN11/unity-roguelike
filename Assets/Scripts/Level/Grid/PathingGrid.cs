using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Class representing a grid used for pathfinding. Assumes the grid cells are squares.
/// </summary>
public class PathingGrid
{
    public const float StraightMoveCost = 10;

    public List<List<GridNode>> Grid { get; set; } = new();
    public Vector2 Position { get; set; }
    public float CellWidth { get; set; }

    /// <returns>The width of the grid in cells</returns>
    public int GetGridWidth()
    {
        return Grid.Count;
    }

    /// <returns>The height of the grid in cells</returns>
    public int GetGridHeight()
    {
        int height = 0;
        if (Grid.Count > 0)
        {
            height = Grid[0].Count;
        }
        return height;
    }

    /// <summary>
    /// Converts the passed world position to a GridNode in the grid.
    /// </summary>
    /// <param name="worldPosition">The world position as a Vector2</param>
    /// <returns>The corresponding GridNode</returns>
    public GridNode WorldToNode(Vector2 worldPosition)
    {
        int xPosition = (int)((worldPosition.x - Position.x) / CellWidth);
        int yPosition = (int)((worldPosition.y - Position.y) / CellWidth);
        return Grid[xPosition][yPosition];
    }

    /// <summary>
    /// Converts the passed GridNode to a position in world space, using the center
    /// of the node.
    /// </summary>
    /// <param name="node">The GridNode</param>
    /// <returns>The corresponding world position as a Vector2</returns>
    public Vector2 NodeToWorld(GridNode node)
    {
        float xPosition = (node.X * CellWidth) + (CellWidth * 0.5f) + Position.x;
        float yPosition = (node.Y * CellWidth) + (CellWidth * 0.5f) + Position.y;
        return new Vector2(xPosition, yPosition);
    }

    /// <summary>
    /// Converts the passed GridNode to a position in world space, using the center
    /// of the node, with the center offset depending on which of the node's edges are blocked.
    /// </summary>
    /// <param name="node">The GridNode</param>
    /// <returns>The corresponding world position with offset as a Vector2</returns>
    public Vector2 NodeToWorldWithOffset(GridNode node)
    {
        float xPosition = (node.X * CellWidth) + (CellWidth * 0.5f) + Position.x;
        float yPosition = (node.Y * CellWidth) + (CellWidth * 0.5f) + Position.y;

        if (!node.TopPassable)
        {
            yPosition -= CellWidth * 0.25f;
        }
        if (!node.BottomPassable)
        {
            yPosition += CellWidth * 0.25f;
        }
        if (!node.LeftPassable)
        {
            xPosition += CellWidth * 0.25f;
        }
        if (!node.RightPassable)
        {
            xPosition -= CellWidth * 0.25f;
        }

        return new Vector2(xPosition, yPosition);
    }
}
