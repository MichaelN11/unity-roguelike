using System;
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

    [SerializeField]
    private Entity singleSpawn;
    public Entity SingleSpawn => singleSpawn;

    [SerializeField]
    private bool isPlayer = false;
    public bool IsPlayer => isPlayer;
}
