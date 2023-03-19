using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Class for building a grid for pathfinding out of collidable tilemaps.
/// </summary>
public class TilemapPathing
{
    private readonly GameObject highlightObject;

    public TilemapPathing(GameObject highlightObject)
    {
        this.highlightObject = highlightObject;
    }

    /// <summary>
    /// Builds the pathing grid used for tilemap pathing from the passed list of tilemaps.
    /// </summary>
    /// <param name="tilemapList">The list of all of the tilemaps in the level</param>
    /// <returns>A PathingGrid object which can be used for pathing AI</returns>
    public PathingGrid Build(List<Tilemap> tilemapList)
    {
        List<Tilemap> unpathableTilemapList = new();
        foreach (Tilemap tilemap in tilemapList)
        {
            if (!IsPassableLayer(tilemap))
            {
                unpathableTilemapList.Add(tilemap);
            }
        }

        PathingGrid pathingGrid = BuildPathingGrid(unpathableTilemapList);
        BuildAdjacentActions(pathingGrid);
        return pathingGrid;
    }

    /// <summary>
    /// Search predicate for finding tilemaps that are passable, which shouldn't be used for pathing.
    /// </summary>
    /// <param name="tilemap">The tilemap to check the layer</param>
    /// <returns>true if the tilemap is using a passable layer</returns>
    private bool IsPassableLayer(Tilemap tilemap)
    {
        return !LayerUtil.IsUnwalkable(tilemap.gameObject.layer);
    }

    /// <summary>
    /// Builds the PathingGrid from the passed Tilemap list. Assumes the tilemap cells are the same size.
    /// </summary>
    /// <param name="tilemapList">The Tilemap list used to build the grid</param>
    /// <returns>The PathingGrid object used for AI pathing</returns>
    private PathingGrid BuildPathingGrid(List<Tilemap> tilemapList)
    {
        PathingGrid pathingGrid = new();
        Vector2 minPosition = Vector2.positiveInfinity;
        Vector2 maxPosition = Vector2.negativeInfinity;
        List<TileBase[]> allTilemapTilesList = new();

        foreach (Tilemap tilemap in tilemapList)
        {
            Bounds localBounds = tilemap.localBounds;
            minPosition.x = (localBounds.min.x < minPosition.x) ? localBounds.min.x : minPosition.x;
            minPosition.y = (localBounds.min.y < minPosition.y) ? localBounds.min.y : minPosition.y;
            maxPosition.x = (localBounds.max.x > maxPosition.x) ? localBounds.max.x : maxPosition.x;
            maxPosition.y = (localBounds.max.y > maxPosition.y) ? localBounds.max.y : maxPosition.y;

            BoundsInt bounds = tilemap.cellBounds;
            TileBase[] allTiles = tilemap.GetTilesBlock(bounds);
            allTilemapTilesList.Add(allTiles);
        }

        pathingGrid.CellWidth = tilemapList[0].cellSize.x;
        pathingGrid.Position = minPosition;

        int gridWidth = (int)((maxPosition.x - minPosition.x) / pathingGrid.CellWidth);
        int gridHeight = (int)((maxPosition.y - minPosition.y) / pathingGrid.CellWidth);

        for (int x = 0; x < gridWidth; x++)
        {
            List<GridNode> column = new();
            pathingGrid.Grid.Add(column);
            for (int y = 0; y < gridHeight; y++)
            {
                GridNode node = new();
                node.X = x;
                node.Y = y;

                node.Passable = IsPositionPassable(node, tilemapList, allTilemapTilesList, pathingGrid);

                column.Add(node);
            }
        }
        return pathingGrid;
    }

    /// <summary>
    /// Determines if the passed position is passable using the passed list of tilemaps and list of all the tilemap tiles.
    /// </summary>
    /// <param name="node">The node position being checked</param>
    /// <param name="tilemapList">The list of tilemaps</param>
    /// <param name="allTilemapTilesList">The list of TileBase[] representing all of the tiles in each tilemap</param>
    /// <param name="pathingGrid">The pathing grid</param>
    /// <returns>true if the grid position is passable</returns>
    private bool IsPositionPassable(GridNode node, List<Tilemap> tilemapList,
        List<TileBase[]> allTilemapTilesList, PathingGrid pathingGrid)
    {
        bool passable = true;
        for (int i = 0; i < tilemapList.Count; i++)
        {
            TileBase[] allTiles = allTilemapTilesList[i];
            Tilemap tilemap = tilemapList[i];
            BoundsInt cellBounds = tilemap.cellBounds;

            Vector2Int tilemapCell = GetTilemapCellEquivalent(tilemap, pathingGrid, node);

            if (tilemapCell.x >= 0
                && tilemapCell.y >= 0
                && tilemapCell.x < cellBounds.size.x
                && tilemapCell.y < cellBounds.size.y)
            {
                int arrayIndex = CalculateTileArrayPosition(tilemapCell.x, tilemapCell.y, cellBounds);
                TileBase tile = allTiles[arrayIndex];
                passable = passable && IsTilePassable(tile);
                if (!passable)
                {
                    if (highlightObject != null)
                    {
                        Object.Instantiate(highlightObject, pathingGrid.NodeToWorld(node), Quaternion.identity);
                    }
                    break;
                }
            }
        }

        return passable;
    }

    /// <summary>
    /// Gets the position of a cell in the passed Tilemap equivalent to the passed GridNode in the passed PathingGrid.
    /// Accounts for tilemaps mirrored in the x direction.
    /// </summary>
    /// <param name="tilemap"></param>
    /// <param name="pathingGrid"></param>
    /// <param name="node"></param>
    /// <returns>The int position of the tilemap cell equivalent to the node</returns>
    private Vector2Int GetTilemapCellEquivalent(Tilemap tilemap, PathingGrid pathingGrid, GridNode node)
    {
        Vector2 tilemapPosition = GetTilemapPosition(tilemap);
        Vector2 tilemapOffset = tilemapPosition - pathingGrid.Position;
        Vector2Int tilemapCellOffset = Vector2Int.FloorToInt(tilemapOffset / pathingGrid.CellWidth);
        Vector2Int tilemapCellPosition = new(node.X - tilemapCellOffset.x, node.Y - tilemapCellOffset.y);

        if (tilemap.transform.lossyScale.x == -1)
        {
            tilemapCellPosition.x = tilemap.size.x - tilemapCellPosition.x - 1;
        }

        return tilemapCellPosition;
    }

    /// <summary>
    /// Gets the position of the tilemap, accounting for mirrored tilemaps in the x direction.
    /// </summary>
    /// <param name="tilemap"></param>
    /// <returns>The position of the tilemap as a Vector2</returns>
    private Vector2 GetTilemapPosition(Tilemap tilemap)
    {
        Vector2 tilemapPosition = Vector2.zero;
        if (tilemap.transform.lossyScale.x == 1)
        {
            tilemapPosition = tilemap.CellToWorld(tilemap.origin);
        } else if (tilemap.transform.lossyScale.x == -1)
        {
            tilemapPosition = tilemap.CellToWorld(tilemap.origin + new Vector3Int(tilemap.size.x, 0, 0));
        }
        return tilemapPosition;
    }

    /// <summary>
    /// Determines if the TileBase is passable. If there is a tile at the position, the node is not passable.
    /// </summary>
    /// <param name="tile">The tile corresponding to the position of the node</param>
    /// <returns>true if tile is null</returns>
    private bool IsTilePassable(TileBase tile)
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
    /// <param name="pathingGrid">The PathingGrid used for AI pathing</param>
    private void BuildAdjacentActions(PathingGrid pathingGrid)
    {
        for (int x = 0; x < pathingGrid.Grid.Count; x++)
        {
            for (int y = 0; y < pathingGrid.Grid[x].Count; y++)
            {
                GridNode node = pathingGrid.Grid[x][y];

                AddAdjacentAction(node, 0, 1, PathingGrid.StraightMoveCost, pathingGrid);
                AddAdjacentAction(node, 0, -1, PathingGrid.StraightMoveCost, pathingGrid);
                AddAdjacentAction(node, 1, 0, PathingGrid.StraightMoveCost, pathingGrid);
                AddAdjacentAction(node, -1, 0, PathingGrid.StraightMoveCost, pathingGrid);
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
    /// <param name="pathingGrid">The PathingGrid used for AI pathing</param>
    private void AddAdjacentAction(GridNode node, int relativeX, int relativeY, float cost, PathingGrid pathingGrid)
    {
        int adjacentX = node.X + relativeX;
        int adjacentY = node.Y + relativeY;

        if (IsCellInGrid(adjacentX, adjacentY, pathingGrid))
        {
            GridNode adjacentNode = pathingGrid.Grid[adjacentX][adjacentY];
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
    /// <param name="pathingGrid">The PathingGrid used for AI pathing</param>
    /// <returns>true if the cell is in the grid</returns>
    private bool IsCellInGrid(int x, int y, PathingGrid pathingGrid)
    {
        return x >= 0
            && x < pathingGrid.Grid.Count
            && y >= 0
            && y < pathingGrid.Grid[0].Count;
    }
}
