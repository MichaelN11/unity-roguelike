using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component class for marking a spawner within an arena.
/// </summary>
public class ArenaSpawner : MonoBehaviour
{
    private void Start()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }
}
