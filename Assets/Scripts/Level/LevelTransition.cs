using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Script for an object that transitions the scene when the player collides with it.
/// </summary>
public class LevelTransition : MonoBehaviour
{
    public bool TransitionEnabled { get; set; } = true;

    [SerializeField]
    public bool isStart = false;

    [SerializeField]
    public bool isEnd = false;

    [SerializeField]
    public bool isWinCondition = false;

    [SerializeField]
    public string newScene;

    [SerializeField]
    public string transitionName;

    [SerializeField]
    private GameObject replacementObject;
    public GameObject ReplacementObject => replacementObject;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && TransitionEnabled)
        {
            if (newScene != null && newScene != "")
            {
                GameManager.Instance.TransitionScene(newScene, transitionName);
            }
            if (isWinCondition)
            {
                GameManager.Instance.WinGame();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            TransitionEnabled = true;
        }
    }
}
