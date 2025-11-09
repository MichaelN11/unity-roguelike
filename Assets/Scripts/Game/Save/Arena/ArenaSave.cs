using System;
using System.Collections.Generic;

/// <summary>
/// Serializable save data for an arena within a level.
/// </summary>
[Serializable]
public class ArenaSave
{
    public bool Completed { get; set; }
    public bool EnemiesSpawned { get; set; }
}
