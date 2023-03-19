using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Singleton component for managing the level. Should be on a Grid object.
/// </summary>
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public PathingGrid PathingGrid { get; set; }

    [SerializeField]
    private Level level;

    [SerializeField]
    private bool highlightPathingGrid = false;

    [SerializeField]
    private GameObject highlightObject;

    private TilemapPathing tilemapPathing;

    private void Awake()
    {
        tilemapPathing = (highlightPathingGrid) ? new(highlightObject) : new(null);

        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        List<LevelTile> unplacedTiles = GetComponentsInChildren<LevelTile>().ToList();
        BuildLevel(unplacedTiles);

        List<Tilemap>  tilemapList = GetComponentsInChildren<Tilemap>().ToList();
        PathingGrid = tilemapPathing.Build(tilemapList);
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    /// <summary>
    /// Builds a level out of the passed list of LevelTiles representing the locations. The tiles
    /// that can be used to build the level are pulled from the Level object. A tile can only be
    /// used once in the level, and they can be randomly mirrored in the x direction.
    /// </summary>
    /// <param name="tileLocations">The list of LevelTiles representing the tile locations</param>
    private void BuildLevel(List<LevelTile> tileLocations)
    {
        if (level != null && tileLocations.Count > 0)
        {
            List<GameObject> placeableTiles = new();
            foreach (GameObject tile in level.Tiles)
            {
                placeableTiles.Add(tile);
            }

            foreach (LevelTile tileLocation in tileLocations)
            {
                if (placeableTiles.Count > 0)
                {
                    PlaceRandomTile(placeableTiles, tileLocation);
                }
                else
                {
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Places a random tile from the list of placeable tiles, removing it from the list. The tile
    /// can be randomly chosen to be mirrored in the x direction.
    /// </summary>
    /// <param name="placeableTiles">The deck of placeable tiles</param>
    /// <param name="tileLocation">The LevelTile representing the tile location</param>
    private void PlaceRandomTile(List<GameObject> placeableTiles, LevelTile tileLocation)
    {
        int randomIndex = Random.Range(0, placeableTiles.Count);
        GameObject tile = placeableTiles[randomIndex];
        placeableTiles.RemoveAt(randomIndex);
        int mirrored = Random.Range(0, 2);
        if (mirrored == 0)
        {
            tileLocation.Place(tile, gameObject);
        } else
        {
            tileLocation.PlaceMirrored(tile, gameObject);
        }
    }
}
