using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Script for an object that transitions the scene when the player collides with it.
/// </summary>
public class LevelTransition : MonoBehaviour
{
    [SerializeField]
    private Object newScene;

    [SerializeField]
    private GameObject endText;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (newScene != null)
            {
                SceneManager.LoadScene(newScene.name);
            }
            if (endText != null)
            {
                endText.SetActive(true);
                Time.timeScale = 0;
            }
        }
    }
}
