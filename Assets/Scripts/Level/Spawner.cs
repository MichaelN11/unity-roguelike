using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A component representing a spawner object used in level generation.
/// </summary>
public class Spawner : MonoBehaviour
{
    [SerializeField]
    private List<Spawnable> spawnables;
    public List<Spawnable> Spawnables => spawnables;
}
