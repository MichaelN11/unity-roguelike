using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class wrapping a list of saved scenes.
/// </summary>
public class SavedScenes
{
    public Dictionary<string, SceneSave> ScenesByName { get; set; } = new();
}
