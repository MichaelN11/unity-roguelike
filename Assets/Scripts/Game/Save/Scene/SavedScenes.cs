using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class wrapping a list of saved scenes.
/// </summary>
[Serializable]
public class SavedScenes
{
    public List<SceneSave> ScenesList { get; set; } = new();

    /// <summary>
    /// Gets the scene for the passed name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public SceneSave GetScene(String name)
    {
        foreach (SceneSave scene in ScenesList)
        {
            if (scene.Name == name)
            {
                return scene;
            }
        }
        return null;
    }

    /// <summary>
    /// Stores the saved scene, overwriting any existing scene with the same name.
    /// </summary>
    /// <param name="sceneSave"></param>
    public void StoreScene(SceneSave sceneSave)
    {
        ScenesList.RemoveAll(scene => scene.Name == sceneSave.Name);
        ScenesList.Add(sceneSave);
    }
}
