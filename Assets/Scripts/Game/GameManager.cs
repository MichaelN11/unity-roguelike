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

    [SerializeField]
    private GameObject player;

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
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SceneManager.sceneLoaded += LoadLevel;
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

    /// <summary>
    /// Subscribed to the scene loaded event. Loads the level.
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="test"></param>
    private void LoadLevel(Scene scene, LoadSceneMode test)
    {
        if (currentTransition != null)
        {
            LevelTransition[] levelTransitions = GameObject.FindObjectsOfType<LevelTransition>();
            foreach (LevelTransition transition in levelTransitions)
            {
                Debug.Log("transition looked at: " + transition.TransitionName + " looking for: " + currentTransition);
                if (transition.TransitionName == currentTransition)
                {
                    Debug.Log("instantiating player");
                    transition.TransitionEnabled = false;
                    Instantiate(player, transition.transform.position, Quaternion.identity);
                    break;
                }
            }
            currentTransition = null;
        }
    }
}
