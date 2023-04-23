using System;
using System.Collections;
using System.Collections.Generic;
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
}
