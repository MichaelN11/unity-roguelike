using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
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

                DeterminePassableEdges(node, pathingGrid);

                if (highlightObject != null)
                {
                    HighlightPathing(pathingGrid, node);
                }

                column.Add(node);
            }
        }
        return pathingGrid;
    }

    /// <summary>
    /// Determines the passable edges of the node. Checks the four corners of the cell
    /// for collisions, and determines that a edge is passable if either of the corners
    /// on the edge are empty.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="pathingGrid"></param>
    private void DeterminePassableEdges(GridNode node, PathingGrid pathingGrid)
    {
        Vector2 worldPosition = pathingGrid.NodeToWorld(node);
        float centerOffset = pathingGrid.CellWidth / 4;
        float colliderWidth = pathingGrid.CellWidth * 0.2f;
        Collider2D bottomRight = Physics2D.OverlapBox(new Vector2(worldPosition.x + centerOffset, worldPosition.y - centerOffset),
            new Vector2(colliderWidth, colliderWidth), 0);
        Collider2D bottomLeft = Physics2D.OverlapBox(new Vector2(worldPosition.x - centerOffset, worldPosition.y - centerOffset),
            new Vector2(colliderWidth, colliderWidth), 0);
        Collider2D topRight = Physics2D.OverlapBox(new Vector2(worldPosition.x + centerOffset, worldPosition.y + centerOffset),
            new Vector2(colliderWidth, colliderWidth), 0);
        Collider2D topLeft = Physics2D.OverlapBox(new Vector2(worldPosition.x - centerOffset, worldPosition.y + centerOffset),
            new Vector2(colliderWidth, colliderWidth), 0);
        node.BottomPassable = (bottomRight == null) || (bottomLeft == null);
        node.TopPassable = (topRight == null) || (topLeft == null);
        node.LeftPassable = (bottomLeft == null) || (topLeft == null);
        node.RightPassable = (bottomRight == null) || (topRight == null);
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
            action.Direction = new Vector2(relativeX, relativeY);
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

    private void HighlightPathing(PathingGrid pathingGrid, GridNode node)
    {
        float borderOffset = pathingGrid.CellWidth * 0.4f;
        if (!node.TopPassable && !node.BottomPassable && !node.LeftPassable && !node.RightPassable)
        {
            Object.Instantiate(highlightObject, pathingGrid.NodeToWorld(node), Quaternion.identity);
        }
        else
        {
            if (!node.BottomPassable)
            {
                GameObject highlight = Object.Instantiate(highlightObject, pathingGrid.NodeToWorld(node), Quaternion.identity);
                highlight.transform.localScale = new Vector3(1, 0.2f);
                highlight.transform.position = new Vector3(highlight.transform.position.x, highlight.transform.position.y - borderOffset);
            }
            if (!node.TopPassable)
            {
                GameObject highlight = Object.Instantiate(highlightObject, pathingGrid.NodeToWorld(node), Quaternion.identity);
                highlight.transform.localScale = new Vector3(1, 0.2f);
                highlight.transform.position = new Vector3(highlight.transform.position.x, highlight.transform.position.y + borderOffset);
            }
            if (!node.LeftPassable)
            {
                GameObject highlight = Object.Instantiate(highlightObject, pathingGrid.NodeToWorld(node), Quaternion.identity);
                highlight.transform.localScale = new Vector3(0.2f, 1);
                highlight.transform.position = new Vector3(highlight.transform.position.x - borderOffset, highlight.transform.position.y);
            }
            if (!node.RightPassable)
            {
                GameObject highlight = Object.Instantiate(highlightObject, pathingGrid.NodeToWorld(node), Quaternion.identity);
                highlight.transform.localScale = new Vector3(0.2f, 1);
                highlight.transform.position = new Vector3(highlight.transform.position.x + borderOffset, highlight.transform.position.y);
            }
        }
    }
}
