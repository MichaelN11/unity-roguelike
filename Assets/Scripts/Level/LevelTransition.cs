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
    private bool isStart = false;
    public bool IsStart => isStart;

    [SerializeField]
    private string newScene;

    [SerializeField]
    private string transitionName;
    public string TransitionName => transitionName;

    [SerializeField]
    private GameObject endText;

    [SerializeField]
    private Sound endSound;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && TransitionEnabled)
        {
            if (newScene != null && newScene != "")
            {
                GameManager.Instance.TransitionScene(newScene, transitionName);
            }
            if (endText != null)
            {
                if (endSound != null)
                {
                    AudioManager.Instance.Play(endSound);
                }
                endText.SetActive(true);
                GameManager.Instance.EndGame();
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
