using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Singleton component for managing the level. Should be on a Grid object.
/// </summary>
public class Level : MonoBehaviour
{
    public static Level Instance { get; private set; }

    public PathingGrid PathingGrid { get; set; }

    private TilemapPathing tilemapPathing = new();
    private List<Tilemap> tilemapList;

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

        tilemapList = GetComponentsInChildren<Tilemap>().ToList();
        PathingGrid = tilemapPathing.Build(tilemapList);
    }

    private void OnDestroy()
    {
        Instance = null;
    }
}
