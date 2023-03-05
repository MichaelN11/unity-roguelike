using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Singleton component for building a grid for pathfinding out of a collidable tilemap.
/// </summary>
public class TilemapPathing : MonoBehaviour
{
    public static TilemapPathing Instance { get; private set; }

    public PathingGrid PathingGrid { get; set; } = new();

    private Tilemap tilemap;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        tilemap = GetComponent<Tilemap>();
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    private void Start()
    {
        BuildPathingGrid(tilemap);
        BuildAdjacentActions();
    }

    /// <summary>
    /// Builds the PathingGrid from the passed Tilemap.
    /// </summary>
    /// <param name="tilemap">The Tilemap used to build the grid</param>
    private void BuildPathingGrid(Tilemap tilemap)
    {
        PathingGrid.CellWidth = tilemap.cellSize.x;
        PathingGrid.Position = tilemap.localBounds.min;

        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);

        for (int x = 0; x < bounds.size.x; x++)
        {
            List<GridNode> column = new();
            PathingGrid.Grid.Add(column);
            for (int y = 0; y < bounds.size.y; y++)
            {
                GridNode node = new();
                node.X = x;
                node.Y = y;

                TileBase tile = allTiles[CalculateTileArrayPosition(x, y, bounds)];
                node.Passable = IsNodePassable(tile);

                column.Add(node);
            }
        }
    }

    /// <summary>
    /// Determines if the node corresponding the to the passed TileBase position
    /// is a passable node. If there is a tile at the position, the node is not passable.
    /// </summary>
    /// <param name="tile">The tile corresponding to the position of the node</param>
    /// <returns>true if tile is null</returns>
    private bool IsNodePassable(TileBase tile)
    {
        return (tile == null);
    }

    /// <summary>
    /// Calculates the position in the 1-dimensional array of tiles, of a tile
    /// at x and y in a 2-dimensional grid with the passed bounds.
    /// </summary>
    /// <param name="x">The x position in the grid</param>
    /// <param name="y">The y position in the grid</param>
    /// <param name="bounds">The bounds of the grid</param>
    /// <returns>The position of the tile in the 1-dimensional array corresponding
    /// to the passed x and y position</returns>
    private int CalculateTileArrayPosition(int x, int y, BoundsInt bounds)
    {
        return x + y * bounds.size.x;
    }

    /// <summary>
    /// Builds the adjacent actions for the nodes in the PathingGrid.
    /// </summary>
    private void BuildAdjacentActions()
    {
        for (int x = 0; x < PathingGrid.Grid.Count; x++)
        {
            for (int y = 0; y < PathingGrid.Grid[x].Count; y++)
            {
                GridNode node = PathingGrid.Grid[x][y];

                AddAdjacentAction(node, 0, 1, PathingGrid.StraightMoveCost);
                AddAdjacentAction(node, 0, -1, PathingGrid.StraightMoveCost);
                AddAdjacentAction(node, 1, 0, PathingGrid.StraightMoveCost);
                AddAdjacentAction(node, -1, 0, PathingGrid.StraightMoveCost);
            }
        }
    }

    /// <summary>
    /// Adds the adjacent action at the passed relative x and y positions to the passed node.
    /// </summary>
    /// <param name="node">The GridNode that the adjacent action will be added to</param>
    /// <param name="relativeX">The relative X position for the action to the node</param>
    /// <param name="relativeY">The relative Y position for the action to the node</param>
    /// <param name="cost">The cost of the action</param>
    private void AddAdjacentAction(GridNode node, int relativeX, int relativeY, float cost)
    {
        int adjacentX = node.X + relativeX;
        int adjacentY = node.Y + relativeY;

        if (IsCellInGrid(adjacentX, adjacentY))
        {
            GridNode adjacentNode = PathingGrid.Grid[adjacentX][adjacentY];
            GridAction action = new();
            action.Node = adjacentNode;
            action.Cost = cost;
            node.AdjacentActions.Add(action);
        }
    }

    /// <summary>
    /// Determines if the cell at the passed x and y position, is in the pathing grid.
    /// </summary>
    /// <param name="x">The x position of the cell</param>
    /// <param name="y">The y position of the cell</param>
    /// <returns>true if the cell is in the grid</returns>
    private bool IsCellInGrid(int x, int y)
    {
        return x >= 0
            && x < PathingGrid.Grid.Count
            && y >= 0
            && y < PathingGrid.Grid[0].Count;
    }
}
