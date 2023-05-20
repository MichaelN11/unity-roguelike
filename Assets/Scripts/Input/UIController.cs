using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

/// <summary>
/// Singleton class for controlling UI input.
/// </summary>
public class UIController : MonoBehaviour
{
    private static UIController Instance { get; set; }

    [SerializeField]
    private GameObject pauseMenu;

    [SerializeField]
    private GameObject optionsMenu;

    [SerializeField]
    private string mainMenuSceneName = "MainMenu";

    private PlayerInputActions inputActions;

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

        inputActions = new PlayerInputActions();
        inputActions.Enable();

        inputActions.UI.Quit.performed += OnPause;
        Resume();
    }

    private void Start()
    {
        if (GameManager.Instance.IsPaused)
        {
            pauseMenu.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        Instance = null;
        inputActions.Dispose();
    }

    /// <summary>
    /// Loads the game.
    /// </summary>
    public void Load()
    {
        GameManager.Instance.LoadGame();
    }

    /// <summary>
    /// Saves the game.
    /// </summary>
    public void Save()
    {
        GameManager.Instance.SaveGame();
    }

    /// <summary>
    /// Resumes the game.
    /// </summary>
    public void Resume()
    {
        optionsMenu.SetActive(false);
        pauseMenu.SetActive(false);
        GameManager.Instance.ResumeGame();
    }

    /// <summary>
    /// Loads the main menu scene.
    /// </summary>
    public void LoadMainMenu()
    {
        GameManager.Instance.TransitionScene(mainMenuSceneName);
    }

    /// <summary>
    /// Shows the options menu and hides the pause menu.
    /// </summary>
    public void ShowOptionsMenu()
    {
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    /// <summary>
    /// Hides the options menu and shows the pause menu.
    /// </summary>
    public void HideOptionsMenu()
    {
        pauseMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }

    /// <summary>
    /// Pauses/unpauses the game when the input is pressed.
    /// </summary>
    /// <param name="ctx">The callback context</param>
    private void OnPause(CallbackContext ctx)
    {
        if (GameManager.Instance.IsPaused)
        {
            Resume();
        } else
        {
            pauseMenu.SetActive(true);
            GameManager.Instance.PauseGame();
        }
    }
}
