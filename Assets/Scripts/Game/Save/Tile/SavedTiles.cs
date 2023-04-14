using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class wrapping a list of saved tiles.
/// </summary>
public class SavedTiles
{
    public List<TileSave> TileList { get; set; } = new();
}
