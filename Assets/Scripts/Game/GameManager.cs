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

    [SerializeField]
    private Entity player;

    private bool loadingSave = false;
    private string currentTransition;
    private string firstScene;
    private readonly JsonFileService jsonFileService = new();

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
        LoadTransition();
        SceneManager.sceneLoaded += SceneLoaded;
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.Initialize();
        }
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
            loadingSave = true;
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

    public void EndGame()
    {
        IsGameOver = true;
        PauseGame();
    }

    /// <summary>
    /// Quits the game.
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Called whenever a scene is loaded. When loading a save, the player is loaded at the
    /// saved position. Otherwise, the player is transitioning between scenes.
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="test"></param>
    private void SceneLoaded(Scene scene, LoadSceneMode test)
    {
        if (loadingSave)
        {
            EntityFactory.LoadPlayer(GameState.Player);
            loadingSave = false;
        }
        else
        {
            LoadTransition();
        }
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.Initialize();
        }
    }

    /// <summary>
    /// Spawns the player at the correct transition object.
    /// </summary>
    private void LoadTransition()
    {
        LevelTransition startTransition = null;
        bool playerSpawned = false;
        LevelTransition[] levelTransitions = GameObject.FindObjectsOfType<LevelTransition>();
        foreach (LevelTransition transition in levelTransitions)
        {
            if (!string.IsNullOrWhiteSpace(currentTransition) && transition.TransitionName == currentTransition)
            {
                SpawnPlayer(transition);
                playerSpawned = true;
                break;
            }
            if (transition.IsStart)
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
        foreach (Chest chest in FindObjectsOfType<Chest>())
        {
            ObjectSave chestSave = new();
            chestSave.Type = ObjectFactory.ChestType;
            chestSave.Position = chest.transform.position;
            if (!chest.Opened && chest.containedItem != null)
            {
                chestSave.InventoryItem = SerializeInventoryItem(chest.containedItem);
            }
            sceneSave.SavedObjects.ObjectList.Add(chestSave);
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
}
