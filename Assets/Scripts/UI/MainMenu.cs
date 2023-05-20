using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * Script for controlling the main menu.
 */
public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private string firstScene = "Forest2x2";

    [SerializeField]
    private GameObject mainMenu;

    [SerializeField]
    private GameObject optionsMenu;

    [SerializeField]
    private Sound music;

    private void Start()
    {
        AudioManager.Instance.Play(music);
    }

    public void NewGame()
    {
        GameManager.Instance.NewGame(firstScene);
    }

    public void Continue()
    {
        GameManager.Instance.LoadGame();
    }

    public void Options()
    {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void BackToMenu()
    {
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
