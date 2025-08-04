using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// Singleton class for managing loading and saving the game.
/// </summary>
public class GameManager : MonoBehaviour
{
    private const string SaveFilePath = "/GameSave.sav";

    public static GameManager Instance { get; private set; }

    public AddressableService AddressableService { get; private set; } = new();
    public SaveObject GameState { get; private set; } = new();
    public bool IsPaused { get; private set; } = false;
    public bool IsGameOver { get; private set; } = false;
    public EntityData CurrentBoss { get; set; }

    [SerializeField]
    private Entity player;

    [SerializeField]
    private Sound winSound;

    [SerializeField]
    private Sound gameOverSound;

    [SerializeField]
    private float gameOverDelay;

    private bool loadingSaveState = false;
    private string currentTransition;
    private string firstScene;
    private readonly JsonFileService jsonFileService = new();
    private bool foundPlayer = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            Initialize();
        }
    }

    private void Initialize()
    {
        firstScene = SceneManager.GetActiveScene().name;
        transform.parent = null;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        IEnumerator coroutine = LoadInitial();
        StartCoroutine(coroutine);
    }

    private IEnumerator LoadInitial()
    {
        yield return AddressableService.Load();
        SetupScene();
        SceneManager.sceneLoaded += SceneLoaded;
    }

    private void Update()
    {
        if (!foundPlayer && PlayerController.Instance != null)
        {
            EntityState playerState = PlayerController.Instance.GetComponent<EntityState>();
            if (playerState != null)
            {
                playerState.OnDeath += OnPlayerDeath;
            }
            foundPlayer = true;
        }
    }

    public void OnPlayerDeath(DeathContext deathContext)
    {
        StartCoroutine(GameOverDelayedCoroutine(deathContext));
    }

    /// <summary>
    /// Starts a new game.
    /// </summary>
    public void NewGame(String firstScene)
    {
        IsGameOver = false;
        GameState = new();
        SceneManager.LoadScene(firstScene);
    }

    /// <summary>
    /// Restarts the game.
    /// </summary>
    public void Restart()
    {
        NewGame(firstScene);
    }

    /// <summary>
    /// Transitions to the scene with the passed name.
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="transitionName"></param>
    public void TransitionScene(string sceneName, string transitionName = "")
    {
        SaveCurrentScene();
        currentTransition = transitionName;
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Saves the current game state to a file.
    /// </summary>
    public void SaveGame()
    {
        SaveCurrentScene();
        jsonFileService.WriteToFile(GameState, Application.persistentDataPath + SaveFilePath);
    }

    /// <summary>
    /// Loads the game state from a file.
    /// </summary>
    public void LoadGame()
    {
        SaveObject savedGame = jsonFileService.ReadFromFile(Application.persistentDataPath + SaveFilePath);
        if (savedGame != null)
        {
            loadingSaveState = true;
            IsGameOver = false;
            GameState = savedGame;
            SceneManager.LoadScene(GameState.CurrentSceneName);
        }
    }

    /// <summary>
    /// Pauses the game.
    /// </summary>
    public void PauseGame()
    {
        IsPaused = true;
        Time.timeScale = 0;
    }

    /// <summary>
    /// Resumes the game.
    /// </summary>
    public void ResumeGame()
    {
        if (!IsGameOver)
        {
            IsPaused = false;
            Time.timeScale = 1;
        }
    }

    public void WinGame()
    {
        if (winSound != null)
        {
            AudioManager.Instance.Play(winSound);
        }
        UIController.Instance.DisplayWinScreen();
        EndGame();
    }

    /// <summary>
    /// Quits the game.
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Quitting the game.");
#if UNITY_EDITOR
        Debug.Log("Stopping Unity editor.");
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
        Debug.Log("Quitting standalone Unity application.");
        Application.Quit();
#elif UNITY_WEBGL
        Debug.Log("Opening about:blank URL.");
        Application.OpenURL("about:blank");
#endif
    }

    private void EndGame()
    {
        IsGameOver = true;
        PauseGame();
    }

    private IEnumerator GameOverDelayedCoroutine(DeathContext deathContext)
    {
        yield return new WaitForSeconds(gameOverDelay);
        GameOver(deathContext);
    }

    private void GameOver(DeathContext deathContext)
    {
        EndGame();
        AudioManager.Instance.Play(gameOverSound);
        UIController.Instance.DisplayDeathScreen(deathContext);
    }

    /// <summary>
    /// When loading a save state, the player is loaded at the
    /// saved position. Otherwise, the player is transitioning between scenes.
    /// </summary>
    private void SetupScene()
    {
        foundPlayer = false;
        if (LevelManager.Instance != null)
        {
            string sceneName = SceneManager.GetActiveScene().name;
            SceneSave loadedScene = GameState.SavedScenes.GetScene(sceneName);
            if (loadedScene != null)
            {
                LoadFromSave(loadedScene);
            }
            LevelManager.Instance.Initialize(loadedScene != null);

            if (loadingSaveState)
            {
                EntityFactory.LoadPlayer(GameState.Player);
                loadingSaveState = false;
            }
            else
            {
                LoadTransition();
            }
        }
    }

    /// <summary>
    /// Called whenever a scene is loaded.
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="test"></param>
    private void SceneLoaded(Scene scene, LoadSceneMode test)
    {
        CurrentBoss = null;
        SetupScene();
    }

    /// <summary>
    /// Spawns the player at the correct transition object.
    /// </summary>
    private void LoadTransition()
    {
        LevelTransition startTransition = null;
        bool playerSpawned = false;
        List<LevelTransition> levelTransitions = LevelManager.Instance.LevelTransitions;
        foreach (LevelTransition transition in levelTransitions)
        {
            if (!string.IsNullOrWhiteSpace(currentTransition) && transition.transitionName == currentTransition)
            {
                SpawnPlayer(transition);
                playerSpawned = true;
                break;
            }
            if (transition.isStart)
            {
                startTransition = transition;
            }
        }
        if (!playerSpawned && startTransition != null)
        {
            SpawnPlayer(startTransition);
        }
        currentTransition = null;
    }

    /// <summary>
    /// Instantiates the player at the passed transition object.
    /// </summary>
    /// <param name="transition"></param>
    private void SpawnPlayer(LevelTransition transition)
    {
        transition.TransitionEnabled = false;
        if (GameState.Player != null)
        {
            GameState.Player.Position = transition.transform.position;
            EntityFactory.LoadPlayer(GameState.Player);
        } else
        {
            EntityFactory.CreatePlayer(player, transition.transform.position);
        }
    }

    /// <summary>
    /// Saves the state of the current scene to the save object.
    /// </summary>
    private void SaveCurrentScene()
    {
        string name = SceneManager.GetActiveScene().name;
        GameState.CurrentSceneName = name;
        SceneSave sceneSave = new();
        sceneSave.Name = name;
        GameState.SavedScenes.StoreScene(sceneSave);

        SavePlayer();
        SaveTiles(sceneSave);
        SaveEntities(sceneSave);
        SaveObjects(sceneSave);
        SaveTransitions(sceneSave);
    }

    /// <summary>
    /// Saves the state of the player.
    /// </summary>
    private void SavePlayer()
    {
        GameState.Player = new();
        if (PlayerController.Instance.TryGetComponent<Damageable>(out Damageable playerDamageable))
        {
            GameState.Player.Health = playerDamageable.CurrentHealth;
        }
        if (PlayerController.Instance.TryGetComponent<EntityData>(out EntityData playerEntityData))
        {
            GameState.Player.Name = playerEntityData.Entity.name;
            GameState.Player.Position = playerEntityData.transform.position;
        }
        if (PlayerController.Instance.TryGetComponent<Inventory>(out Inventory playerInventory))
        {
            GameState.Player.InventoryItems = new List<InventoryItemSave>();
            foreach (InventoryItem inventoryItem in playerInventory.Items)
            {
                GameState.Player.InventoryItems.Add(SerializeInventoryItem(inventoryItem));
            }
        }
    }

    /// <summary>
    /// Saves the tiles in the scene.
    /// </summary>
    /// <param name="sceneSave"></param>
    private void SaveTiles(SceneSave sceneSave)
    {
        foreach (LevelTile levelTile in FindObjectsOfType<LevelTile>())
        {
            TileSave tileSave = new();
            tileSave.Name = levelTile.TileName;
            tileSave.IsFlipped = levelTile.IsFlipped;
            tileSave.Position = levelTile.transform.position;
            sceneSave.SavedTiles.TilesList.Add(tileSave);
        }
    }

    /// <summary>
    /// Saves the entities in the scene.
    /// </summary>
    /// <param name="sceneSave"></param>
    private void SaveEntities(SceneSave sceneSave)
    {
        foreach (EntityController entityController in FindObjectsOfType<EntityController>())
        {
            if (!entityController.CompareTag("Player") && entityController.GetComponent<EntityState>().ActionState != ActionState.Dead)
            {
                EntitySave entitySave = new();
                entitySave.Name = entityController.GetComponent<EntityData>().Entity.name;
                entitySave.Position = entityController.transform.position;
                sceneSave.SavedEntities.EntityList.Add(entitySave);
            }
        }
    }

    private void SaveObjects(SceneSave sceneSave)
    {
        foreach (LevelObject levelObject in FindObjectsOfType<LevelObject>())
        {
            if (levelObject.Type != null)
            {
                ObjectSave objectSave = new();
                objectSave.Type = levelObject.Type;
                objectSave.Position = levelObject.transform.position;
                if (levelObject.containedItem.Item != null && levelObject.containedItem.Amount > 0)
                {
                    objectSave.InventoryItem = SerializeInventoryItem(levelObject.containedItem);
                }
                sceneSave.SavedObjects.ObjectList.Add(objectSave);
            }
        }
    }

    private InventoryItemSave SerializeInventoryItem(InventoryItem inventoryItem)
    {
        return new InventoryItemSave()
        {
            Name = inventoryItem.Item.name,
            Amount = inventoryItem.Amount
        };
    }

    private void SaveTransitions(SceneSave sceneSave)
    {
        foreach (LevelTransition transition in FindObjectsOfType<LevelTransition>())
        {
            SpriteRenderer spriteRenderer = transition.GetComponent<SpriteRenderer>();
            TransitionSave transitionSave = new();
            transitionSave.Position = transition.transform.position;
            transitionSave.Rotation = transition.transform.rotation.eulerAngles.z;
            transitionSave.IsStart = transition.isStart;
            transitionSave.IsEnd = transition.isEnd;
            transitionSave.IsWinCondition = transition.isWinCondition;
            transitionSave.NewScene = transition.newScene;
            transitionSave.TransitionName = transition.transitionName;
            transitionSave.IsVisible = spriteRenderer.enabled;
            sceneSave.SavedTransitions.TransitionList.Add(transitionSave);
        }
    }

    private void LoadFromSave(SceneSave sceneSave)
    {
        List<LevelTile> unplacedTiles = LevelManager.Instance.GetComponentsInChildren<LevelTile>().ToList();
        LoadLevel(unplacedTiles, sceneSave);

        // Sync the transforms to account for flipped tiles when building the pathing grid.
        Physics2D.SyncTransforms();

        LoadEntities(sceneSave);
        LoadObjects(sceneSave);
        LoadTransitions(sceneSave);
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
        foreach (GameObject tile in LevelManager.Instance.Level.Tiles)
        {
            Debug.Log("Level tile: " + tile.name + " Saved tile: " + tileSave.Name);
            if (tile.name == tileSave.Name)
            {
                loadedTile = tile;
                break;
            }
        }
        if (loadedTile != null)
        {
            TileObjects tileObjects;
            if (tileSave.IsFlipped)
            {
                tileObjects = levelTile.PlaceMirrored(loadedTile, LevelManager.Instance.gameObject);
            }
            else
            {
                tileObjects = levelTile.Place(loadedTile, LevelManager.Instance.gameObject);
            }
            foreach (GameObject gameObject in tileObjects.ObjectList)
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

    private void LoadObjects(SceneSave sceneSave)
    {
        foreach (ObjectSave objectSave in sceneSave.SavedObjects.ObjectList)
        {
            ObjectFactory.LoadObject(objectSave);
        }
    }

    private void LoadTransitions(SceneSave sceneSave)
    {
        // Clear out all of the initial transitions in the scene before loading them.
        LevelTransition[] initialTransitions = GameObject.FindObjectsOfType<LevelTransition>();
        foreach (LevelTransition transition in initialTransitions)
        {
            Destroy(transition.gameObject);
        }

        GameObject transitionPrefab = ResourceManager.Instance.TransitionObject;
        foreach (TransitionSave transitionSave in sceneSave.SavedTransitions.TransitionList)
        {
            GameObject transitionObject = Instantiate(transitionPrefab, transitionSave.Position,
                Quaternion.Euler(0, 0, transitionSave.Rotation));
            LevelTransition transition = transitionObject.GetComponent<LevelTransition>();
            SpriteRenderer spriteRenderer = transitionObject.GetComponent<SpriteRenderer>();
            transition.isStart = transitionSave.IsStart;
            transition.isEnd = transitionSave.IsEnd;
            transition.isWinCondition = transitionSave.IsWinCondition;
            transition.newScene = transitionSave.NewScene;
            transition.transitionName = transitionSave.TransitionName;
            spriteRenderer.enabled = transitionSave.IsVisible;

            LevelManager.Instance.LevelTransitions.Add(transition);
            if (transition.isStart)
            {
                LevelManager.Instance.StartTransition = transition;
            }
        }
    }
}
