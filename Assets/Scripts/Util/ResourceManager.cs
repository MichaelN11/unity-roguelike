using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton class for managing resources.
/// </summary>
public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    public Material FlashMaterial { get; private set; }

    [SerializeField]
    private string flashMaterial = "Flash";

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

        transform.parent = null;
        DontDestroyOnLoad(gameObject);
        LoadResources();
    }

    /// <summary>
    /// Loads the static resources from the Resources folder.
    /// </summary>
    private void LoadResources()
    {
        FlashMaterial = (Material) Resources.Load(flashMaterial);
    }
}
