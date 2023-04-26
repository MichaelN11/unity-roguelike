using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// POCO representing a saved instance of the game state.
/// </summary>
[Serializable]
public class SaveObject
{
    public string CurrentSceneName { get; set; }
    public EntitySave Player { get; set; }
    public SavedScenes SavedScenes { get; set; } = new();
}
