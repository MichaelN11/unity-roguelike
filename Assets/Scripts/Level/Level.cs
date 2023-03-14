using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable object representing a level.
/// </summary>
[CreateAssetMenu(menuName = "Game Data/Level")]
public class Level : ScriptableObject
{
    [SerializeField]
    private List<GameObject> tiles;
    public List<GameObject> Tiles => tiles;
}
