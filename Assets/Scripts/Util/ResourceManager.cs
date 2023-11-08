using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton class for managing resources.
/// </summary>
public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    [SerializeField]
    private Material flashMaterial;
    public Material FlashMaterial => flashMaterial;

    [SerializeField]
    private GameObject itemPickupObject;
    public GameObject ItemPickupObject => itemPickupObject;

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
    }
}
