using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// POCO representing a saved scene.
/// </summary>
public class SceneSave
{
    public string Name { get; set; }
    public SavedTiles SavedTiles { get; set; } = new();
    public SavedEntities SavedEntities { get; set; } = new();
}
