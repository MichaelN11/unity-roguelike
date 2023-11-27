using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// POCO representing a saved scene.
/// </summary>
[Serializable]
public class SceneSave
{
    public string Name { get; set; }
    public SavedTiles SavedTiles { get; set; } = new();
    public SavedEntities SavedEntities { get; set; } = new();
    public SavedObjects SavedObjects { get; set; } = new();
}
