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

    private TilemapPathing tilemapPathing = new();

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

        List<LevelTile> unplacedTiles = GetComponentsInChildren<LevelTile>().ToList();
        BuildLevel(unplacedTiles);

        List<Tilemap>  tilemapList = GetComponentsInChildren<Tilemap>().ToList();
        PathingGrid = tilemapPathing.Build(tilemapList);
    }

    private void OnDestroy()
    {
        Instance = null;
    }

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
                    int randomIndex = Random.Range(0, placeableTiles.Count);
                    GameObject tile = placeableTiles[randomIndex];
                    placeableTiles.RemoveAt(randomIndex);
                    tileLocation.Place(tile, gameObject);
                } else
                {
                    break;
                }
            }
        }
    }
}
