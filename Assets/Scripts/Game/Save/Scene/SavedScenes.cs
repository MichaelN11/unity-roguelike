using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class wrapping a list of saved scenes.
/// </summary>
public class SavedScenes
{
    public List<SceneSave> SceneList { get; set; } = new();
}
