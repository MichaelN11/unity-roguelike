using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;

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
    private GameObject quitButton;

    [SerializeField]
    private GameObject classSelectMenu;

    [SerializeField]
    private Sound music;

    [SerializeField]
    private Sound buttonClick;
    [SerializeField]
    private Sound checkboxSelect;

    private CharacterClass selectedClass = CharacterClass.Soldier;

    private void Awake()
    {
        // WebGL does not need a quit button for playing in a web browser.
#if UNITY_WEBGL && !UNITY_EDITOR
        if (quitButton != null)
        {
            quitButton.SetActive(false);
        }
#endif
    }

    private void Start()
    {
        AudioManager.Instance.Play(music);
        AudioManager.Instance.StopAmbience();
    }

    public void StartGame()
    {
        GameManager.Instance.NewGame(firstScene, selectedClass);
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

    public void ClassSelectScreen()
    {
        mainMenu.SetActive(false);
        classSelectMenu.SetActive(true);
    }

    public void BackToMenu()
    {
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
        classSelectMenu.SetActive(false);
    }

    public void Quit()
    {
        GameManager.Instance.QuitGame();
    }

    public void SelectSoldier()
    {
        selectedClass = CharacterClass.Soldier;
    }

    public void SelectHunter()
    {
        selectedClass = CharacterClass.Hunter;
    }

    public void PlayButtonClick()
    {
        AudioManager.Instance.Play(buttonClick);
    }
    
    public void PlayCheckboxSelect()
    {
        AudioManager.Instance.Play(checkboxSelect);
    }
}
