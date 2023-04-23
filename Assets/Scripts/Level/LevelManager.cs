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

    private LevelBounds levelBounds;
    public LevelBounds LevelBounds => levelBounds;

    [SerializeField]
    private Level level;

    [SerializeField]
    private float minimumSpawnDistanceFromPlayer = 8;

    [SerializeField]
    private bool highlightPathingGrid = false;

    [SerializeField]
    private GameObject highlightObject;

    [SerializeField]
    private bool spawnObjects = true;

    private TilemapPathing tilemapPathing;
    private List<TileObjects> tileObjectsList;

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
        tileObjectsList = BuildLevel(unplacedTiles);

        List<Tilemap>  tilemapList = GetComponentsInChildren<Tilemap>().ToList();
        PathingGrid = tilemapPathing.Build(tilemapList);

        levelBounds = GetComponentInChildren<LevelBounds>();
    }

    private void Start()
    {
        SpawnObjects(tileObjectsList, level);
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    /// <summary>
    /// Gets the spawners from the tile objects list, and decides which objects to spawn in each tile.
    /// </summary>
    /// <param name="tileObjectsList">The list of TileObjects containing spawners</param>
    /// <param name="level">The level scriptable object</param>
    private void SpawnObjects(List<TileObjects> tileObjectsList, Level level)
    {
        foreach (TileObjects tileObjects in tileObjectsList)
        {
            int spawnedObjectCount = 0;
            while (spawnedObjectCount < level.MaxEnemiesPerTile && tileObjects.objectList.Count > 0)
            {
                int randomIndex = Random.Range(0, tileObjects.objectList.Count);
                GameObject tileObject = tileObjects.objectList[randomIndex];
                Spawner spawner = tileObject.GetComponent<Spawner>();
                if (spawner != null)
                {
                    if (spawnObjects && !IsTooCloseToPlayer(spawner.transform))
                    {
                        SpawnObject(spawner, level);
                        ++spawnedObjectCount;
                    }
                    Destroy(tileObject);
                    tileObjects.objectList.RemoveAt(randomIndex);
                }
            }
            foreach (GameObject tileObject in tileObjects.objectList)
            {
                if (tileObject.GetComponent<Spawner>() != null)
                {
                    Destroy(tileObject);
                }
            }
        }
    }

    /// <summary>
    /// Spawns an object from the passed spawner.
    /// </summary>
    /// <param name="spawner">The spawner component</param>
    /// <param name="level">The level scriptable object</param>
    private void SpawnObject(Spawner spawner, Level level)
    {
        List<Entity> spawnableObjects = new();
        foreach (Spawnable spawnable in spawner.Spawnables)
        {
            switch (spawnable)
            {
                case Spawnable.MeleeEnemy:
                    spawnableObjects.AddRange(level.MeleeEnemies);
                    break;
                case Spawnable.RangedEnemy:
                    spawnableObjects.AddRange(level.RangedEnemies);
                    break;
            }
        }
        if (spawnableObjects.Count > 0)
        {
            int randomIndex = Random.Range(0, spawnableObjects.Count);
            EntityFactory.CreateEnemy(spawnableObjects[randomIndex], spawner.transform.position);
        }
    }

    /// <summary>
    /// Builds a level out of the passed list of LevelTiles representing the locations. The tiles
    /// that can be used to build the level are pulled from the Level object. A tile can only be
    /// used once in the level, and they can be randomly mirrored in the x direction.
    /// </summary>
    /// <param name="tileLocations">The list of LevelTiles representing the tile locations</param>
    /// <returns>a List of TileObjects built with the level</returns>
    private List<TileObjects> BuildLevel(List<LevelTile> tileLocations)
    {
        List<TileObjects> tileObjectsList = new();
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
                    TileObjects tileObjects = PlaceRandomTile(placeableTiles, tileLocation);
                    tileObjectsList.Add(tileObjects);
                }
                else
                {
                    break;
                }
            }
        }
        return tileObjectsList;
    }

    /// <summary>
    /// Places a random tile from the list of placeable tiles, removing it from the list. The tile
    /// can be randomly chosen to be mirrored in the x direction.
    /// </summary>
    /// <param name="placeableTiles">The deck of placeable tiles</param>
    /// <param name="tileLocation">The LevelTile representing the tile location</param>
    /// <returns>TileObjects that were created with the tile</returns>
    private TileObjects PlaceRandomTile(List<GameObject> placeableTiles, LevelTile tileLocation)
    {
        int randomIndex = Random.Range(0, placeableTiles.Count);
        GameObject tile = placeableTiles[randomIndex];
        placeableTiles.RemoveAt(randomIndex);
        int mirrored = Random.Range(0, 2);
        if (mirrored == 0)
        {
            return tileLocation.Place(tile, gameObject);
        } else
        {
            return tileLocation.PlaceMirrored(tile, gameObject);
        }
    }

    /// <summary>
    /// Determines if the passed transform object is too close to the player. This is to prevent enemies from
    /// spawning next to the player.
    /// </summary>
    /// <param name="transform"></param>
    /// <returns>true if the passed transform is too lose to the player's position</returns>
    private bool IsTooCloseToPlayer(Transform transform)
    {
        bool isTooClose = false;
        PlayerController playerController = PlayerController.Instance;
        if (playerController != null)
        {
            float distance = Vector2.Distance(playerController.transform.position, transform.position);
            isTooClose = distance <= minimumSpawnDistanceFromPlayer;
        }
        return isTooClose;
    }
}
