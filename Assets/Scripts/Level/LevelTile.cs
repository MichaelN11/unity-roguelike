using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component for a tile in a level.
/// </summary>
public class LevelTile : MonoBehaviour
{
    /// <summary>
    /// Places the object as the tile.
    /// </summary>
    /// <param name="gameObject">The tile object being placed</param>
    /// <param name="grid">The Grid object being used by the level</param>
    public void Place(GameObject tile, GameObject grid)
    {
        Object.Instantiate(tile, transform.position, transform.rotation, grid.transform);
        Destroy(gameObject);
    }
}
