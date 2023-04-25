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
    public static GameManager Instance { get; private set; }

    public SaveObject GameState { get; private set; } = new();

    [SerializeField]
    private Entity player;

    private string currentTransition;
    private string firstScene;

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
        LoadTransition();
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SceneManager.sceneLoaded += SceneLoaded;
    }

    /// <summary>
    /// Restarts the game.
    /// </summary>
    public void Restart()
    {
        GameState = new();
        Time.timeScale = 1;
        SceneManager.LoadScene(firstScene);
    }

    /// <summary>
    /// Transitions to the scene with the passed name.
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="transitionName"></param>
    public void TransitionScene(string sceneName, string transitionName)
    {
        SaveCurrentScene();
        currentTransition = transitionName;
        SceneManager.LoadScene(sceneName);
    }

    private void SceneLoaded(Scene scene, LoadSceneMode test)
    {
        LoadTransition();
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
        SceneSave sceneSave = new();
        GameState.SavedScenes.ScenesByName[name] = sceneSave;
        sceneSave.Name = name;

        SavePlayer();
        SaveTiles(sceneSave);
        SaveEntities(sceneSave);
    }

    /// <summary>
    /// Saves the state of the player.
    /// </summary>
    private void SavePlayer()
    {
        GameState.Player = new();
        Damageable playerDamageable = PlayerController.Instance.GetComponent<Damageable>();
        if (playerDamageable != null)
        {
            GameState.Player.Health = playerDamageable.CurrentHealth;
        }
        EntityData playerEntityData = PlayerController.Instance.GetComponent<EntityData>();
        if (playerEntityData != null)
        {
            GameState.Player.Name = playerEntityData.Entity.name;
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
            sceneSave.SavedTiles.TilesByPosition[tileSave.Position] = tileSave;
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
}
