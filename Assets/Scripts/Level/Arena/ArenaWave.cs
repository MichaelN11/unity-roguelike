using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

/// <summary>
/// Class for defining a wave of enemies in an arena.
/// </summary>
[Serializable]
public class ArenaWave
{
    /// <summary>
    /// Maximum number of enemies to spawn initially in this wave. Can't be greater than the number of arena spawners.
    /// </summary>
    public int MaxInitialEnemies { get; set; } = int.MaxValue;
    /// <summary>
    /// Time interval between enemy spawns in this wave. If less than or equal to zero, enemies will not spawn over time.
    /// </summary>
    public float EnemySpawnInterval { get; set; } = -1f;
    /// <summary>
    /// Maximum number of enemies allowed to be alive at once in this wave.
    /// </summary>
    public int MaxEnemiesAtOnce { get;  set; } = int.MaxValue;

    public List<EntitySpawn> EnemySpawns;
}
