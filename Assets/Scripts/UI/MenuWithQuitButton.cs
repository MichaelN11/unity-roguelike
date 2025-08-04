using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component for managing a UI menu with a quit button.
/// The menu is shown without the quit button if it's running in WebGL.
/// </summary>
public class MenuWithQuitButton : MonoBehaviour
{
    [SerializeField]
    private GameObject buttonsWithQuit;

    [SerializeField]
    private GameObject buttonsWithoutQuit;

    private void Awake()
    {
        // WebGL does not need a quit button for playing in a web browser.
#if UNITY_WEBGL && !UNITY_EDITOR
        if (buttonsWithQuit != null && buttonsWithoutQuit != null)
        {
            buttonsWithQuit.SetActive(false);
            buttonsWithoutQuit.SetActive(true);
        }
#endif
    }
}
