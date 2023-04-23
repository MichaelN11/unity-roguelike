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

    public SaveObject GameState { get; private set; }

    [SerializeField]
    private Entity player;

    private string currentTransition;

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
        LoadTransition();
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SceneManager.sceneLoaded += SceneLoaded;
    }

    /// <summary>
    /// Transitions to the scene with the passed name, instantiates the player in the scene using the 
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="transitionName"></param>
    public void TransitionScene(string sceneName, string transitionName)
    {
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
        EntityFactory.CreatePlayer(player, transition.transform.position);
    }
}
