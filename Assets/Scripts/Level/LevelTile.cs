using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Component for a tile in a level.
/// </summary>
public class LevelTile : MonoBehaviour
{
    [SerializeField]
    private float tileWidth = 16;

    /// <summary>
    /// Places the object as the tile. Returns a list of GameObjects that were created with the tile.
    /// </summary>
    /// <param name="gameObject">The tile object being placed</param>
    /// <param name="grid">The Grid object being used by the level</param>
    /// <returns>TileObjects that were created with the tile</returns>
    public TileObjects Place(GameObject tile, GameObject grid)
    {
        GameObject placedObject = Object.Instantiate(tile, transform.position, transform.rotation, grid.transform);
        Destroy(gameObject);
        return DetachNonTilemaps(placedObject.transform);
    }

    /// <summary>
    /// Places the object as the tile, mirrored in the x direction. Returns a list of GameObjects that were created with the tile.
    /// </summary>
    /// <param name="gameObject">The tile object being placed</param>
    /// <param name="grid">The Grid object being used by the level</param>
    /// <returns>TileObjects that were created with the tile</returns>
    public TileObjects PlaceMirrored(GameObject tile, GameObject grid)
    {
        GameObject placedObject = Instantiate(tile, transform.position, transform.rotation, grid.transform);
        Destroy(gameObject);
        MirrorTileInXDirection(placedObject.transform);
        return DetachNonTilemaps(placedObject.transform);
    }

    /// <summary>
    /// Detach children that don't have the tilemap component. If the tilemap was mirrored, unmirror the children.
    /// Returns a list of the detached objects.
    /// </summary>
    /// <param name="parent"></param>
    /// <returns>TileObjects that were detached</returns>
    private TileObjects DetachNonTilemaps(Transform parent)
    {
        TileObjects tileObjects = new();
        // Loop backward to make it safe to detach child in the loop
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Transform child = parent.GetChild(i);
            if (!child.GetComponent<Tilemap>())
            {
                tileObjects.objectList.Add(child.gameObject);
                child.parent = null;
                if (parent.transform.localScale.x == -1)
                {
                    MirrorScaleX(child.transform);
                }
            }
        }
        return tileObjects;
    }

    /// <summary>
    /// Mirrors the passed tile's transform in the x direction. Accounts for the tile position change.
    /// </summary>
    /// <param name="transform">The game object's transform component</param>
    private void MirrorTileInXDirection(Transform transform)
    {
        MirrorScaleX(transform);

        Vector3 position = transform.position;
        position.x += tileWidth;
        transform.position = position;
    }

    /// <summary>
    /// Mirrors the X scale of the passed transform.
    /// </summary>
    /// <param name="transform"></param>
    private void MirrorScaleX(Transform transform)
    {
        Vector3 mirroredScale = transform.localScale;
        mirroredScale.x *= -1;
        transform.localScale = mirroredScale;
    }
}
