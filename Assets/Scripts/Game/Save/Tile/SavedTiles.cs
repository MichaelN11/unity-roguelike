using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class wrapping a list of saved tiles.
/// </summary>
[Serializable]
public class SavedTiles
{
    public List<TileSave> TilesList { get; set; } = new();
    private Dictionary<Vector2Int, TileSave> tilesByPosition;

    /// <summary>
    /// Gets the tile for the passed position.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public TileSave GetTile(Vector2 position)
    {
        if (tilesByPosition == null)
        {
            tilesByPosition = new();
            foreach (TileSave tile in TilesList)
            {
                tilesByPosition.Add(Vector2Int.RoundToInt(tile.Position), tile);
            }
        }
        Vector2Int roundedPosition = Vector2Int.RoundToInt(position);
        tilesByPosition.TryGetValue(roundedPosition, out TileSave tileSave);
        return tileSave;
    }
}
