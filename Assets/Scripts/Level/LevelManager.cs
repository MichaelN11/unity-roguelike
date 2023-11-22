using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pathfinding;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

/// <summary>
/// Singleton component for managing the level. Should be on a Grid object.
/// </summary>
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public event System.Action OnLevelInitialized;

    private LevelBounds levelBounds;
    public LevelBounds LevelBounds => levelBounds;

    [SerializeField]
    private Level level;

    [SerializeField]
    private float minimumSpawnDistanceFromPlayer = 8;

    [SerializeField]
    private bool spawnObjects = true;

    private List<TileObjects> tileObjectsList;
    private SceneSave loadedScene;
    private bool sendInitializedEvent = false;

    public void Initialize()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        loadedScene = GameManager.Instance.GameState.SavedScenes.GetScene(sceneName);
        List<LevelTile> unplacedTiles = GetComponentsInChildren<LevelTile>().ToList();
        if (loadedScene == null)
        {
            SpawnStaticObjects();
            tileObjectsList = BuildLevel(unplacedTiles);
        }
        else
        {
            DestroyStaticSpawners();
            LoadLevel(unplacedTiles, loadedScene);
        }

        // Sync the transforms to account for flipped tiles when building the pathing grid.
        Physics2D.SyncTransforms();

        if (loadedScene == null)
        {
            SpawnObjects(tileObjectsList, level);
        }
        else
        {
            LoadEntities(loadedScene);
        }

        if (levelBounds != null)
        {
            BuildAStarGrid();
        }

        if (level != null && level.Music != null)
        {
            AudioManager.Instance.Play(level.Music);
        }
        sendInitializedEvent = true;
    }

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

        levelBounds = GetComponentInChildren<LevelBounds>();
    }

    private void Update()
    {
        if (sendInitializedEvent)
        {
            sendInitializedEvent = false;
            OnLevelInitialized?.Invoke();
        }
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    private void BuildAStarGrid()
    {
        AstarData data = AstarPath.active.data;

        GridGraph gg = data.AddGraph(typeof(GridGraph)) as GridGraph;

        gg.is2D = true;
        gg.collision.use2D = true;

        float nodeSize = 0.25f;
        int width = (int)(levelBounds.Bounds.size.x * (1/nodeSize));
        int depth = (int)(levelBounds.Bounds.size.y * (1/nodeSize));
        gg.center = levelBounds.Bounds.center;
        gg.SetDimensions(width, depth, nodeSize);

        gg.cutCorners = false;

        gg.collision.diameter = 1.5f;
        gg.collision.mask = LayerUtil.GetUnwalkableLayerMask();

        AstarPath.active.Scan();
    }

    /// <summary>
    /// Spawns objects that are pre-placed in the level, not part of a tile.
    /// </summary>
    private void SpawnStaticObjects()
    {
        foreach (Spawner spawner in FindObjectsOfType<Spawner>())
        {
            SpawnEntity(spawner, level);
            Destroy(spawner.gameObject);
        }
    }

    /// <summary>
    /// Destroys pre-placed spawn objects.
    /// </summary>
    private void DestroyStaticSpawners()
    {
        foreach (Spawner spawner in FindObjectsOfType<Spawner>())
        {
            Destroy(spawner.gameObject);
        }
    }

    /// <summary>
    /// Gets the spawners from the tile objects list, and decides which objects to spawn in each tile.
    /// </summary>
    /// <param name="tileObjectsList">The list of TileObjects containing spawners</param>
    /// <param name="level">The level scriptable object</param>
    private void SpawnObjects(List<TileObjects> tileObjectsList, Level level)
    {
        List<List<ChestSpawner>> tilesWithChests = new();
        foreach (TileObjects tileObjects in tileObjectsList)
        {
            List<Spawner> entitySpawners = new();
            List<ChestSpawner> chestSpawners = new();

            foreach (GameObject gameObject in tileObjects.objectList)
            {
                if (gameObject.TryGetComponent<Spawner>(out Spawner spawner))
                {
                    entitySpawners.Add(spawner);
                } else if (gameObject.TryGetComponent<ChestSpawner>(out ChestSpawner chestSpawner))
                {
                    chestSpawners.Add(chestSpawner);
                }
            }
            if (chestSpawners.Count > 0)
            {
                tilesWithChests.Add(chestSpawners);
            }
            SpawnEntities(entitySpawners, level);
        }
        if (level != null)
        {
            SpawnChests(tilesWithChests, level);
        }
    }

    private void SpawnEntities(List<Spawner> entitySpawners, Level level)
    {
        int spawnedEntityCount = 0;
        int enemiesPerTile = Random.Range(level.MinEnemiesPerTile, level.MaxEnemiesPerTile + 1);
        while (spawnedEntityCount < enemiesPerTile && entitySpawners.Count > 0)
        {
            int randomIndex = Random.Range(0, entitySpawners.Count);
            Spawner spawner = entitySpawners[randomIndex];
            if (spawnObjects && !IsTooCloseToPlayer(spawner.transform))
            {
                SpawnEntity(spawner, level);
                ++spawnedEntityCount;
            }
            Destroy(spawner.gameObject);
            entitySpawners.RemoveAt(randomIndex);
        }
        foreach (Spawner spawner in entitySpawners)
        {
            Destroy(spawner.gameObject);
        }
    }

    /// <summary>
    /// Spawns an object from the passed spawner.
    /// </summary>
    /// <param name="spawner">The spawner component</param>
    /// <param name="level">The level scriptable object</param>
    private void SpawnEntity(Spawner spawner, Level level)
    {
        if (spawner.SingleSpawn != null)
        {
            if (spawner.IsPlayer)
            {
                EntityFactory.CreatePlayer(spawner.SingleSpawn, spawner.transform.position);
            }
            else
            {
                EntityFactory.CreateEnemy(spawner.SingleSpawn, spawner.transform.position);
            }          
        } else
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
    }

    private void SpawnChests(List<List<ChestSpawner>> tilesWithChests, Level level)
    {
        int spawnedChestCount = 0;
        int chestsPerLevel = Random.Range(level.MinChestsPerLevel, level.MaxChestsPerLevel + 1);
        while (spawnedChestCount < chestsPerLevel && tilesWithChests.Count > 0)
        {
            int randomTileIndex = Random.Range(0, tilesWithChests.Count);
            List<ChestSpawner> chestSpawners = tilesWithChests[randomTileIndex];
            if (spawnObjects)
            {
                SpawnChest(chestSpawners, level);
                ++spawnedChestCount;
            }
            foreach (ChestSpawner chestSpawner in chestSpawners)
            {
                Destroy(chestSpawner.gameObject);
            }
        }
        foreach (List<ChestSpawner> remainingTile in tilesWithChests)
        {
            foreach (ChestSpawner chestSpawner in remainingTile)
            {
                Destroy(chestSpawner.gameObject);
            }
        }
    }

    private void SpawnChest(List<ChestSpawner> chestSpawners, Level level)
    {
        int randomChest = Random.Range(0, chestSpawners.Count);
        ChestSpawner chestSpawner = chestSpawners[randomChest];
        GameObject newObject = Instantiate(ResourceManager.Instance.ChestObject, chestSpawner.transform.position, Quaternion.identity);
        if (newObject.TryGetComponent<Chest>(out Chest chest))
        {
            chest.containedItem.Item = GameManager.Instance.AddressableService.RetrieveItem("HealingPotion");
            chest.containedItem.Amount = 2;
        } else
        {
            Debug.Log("ResourceManager chest object has no chest component");
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
        } else
        {
            Debug.Log("Player distance check in level generation not working");
        }
        return isTooClose;
    }

    /// <summary>
    /// Loads the level tiles from the saved scene.
    /// </summary>
    /// <param name="levelTiles"></param>
    /// <param name="sceneSave"></param>
    private void LoadLevel(List<LevelTile> levelTiles, SceneSave sceneSave)
    {
        foreach (LevelTile levelTile in levelTiles)
        {
            TileSave tileSave = sceneSave.SavedTiles.GetTile(levelTile.transform.position);
            if (tileSave != null)
            {
                LoadTile(levelTile, tileSave);
            }
        }
    }

    /// <summary>
    /// Loads the saved tile.
    /// </summary>
    /// <param name="levelTile"></param>
    /// <param name="tileSave"></param>
    private void LoadTile(LevelTile levelTile, TileSave tileSave)
    {
        GameObject loadedTile = null;
        foreach (GameObject tile in level.Tiles)
        {
            if (tile.name == tileSave.Name)
            {
                loadedTile = tile;
            }
        }
        if (loadedTile != null)
        {
            TileObjects tileObjects;
            if (tileSave.IsFlipped)
            {
                tileObjects = levelTile.PlaceMirrored(loadedTile, gameObject);
            }
            else
            {
                tileObjects = levelTile.Place(loadedTile, gameObject);
            }
            foreach (GameObject gameObject in tileObjects.objectList)
            {
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// Loads the entities from the saved scene.
    /// </summary>
    /// <param name="sceneSave"></param>
    private void LoadEntities(SceneSave sceneSave)
    {
        foreach (EntitySave entitySave in sceneSave.SavedEntities.EntityList)
        {
            EntityFactory.LoadEnemy(entitySave);
        }
    }
}
