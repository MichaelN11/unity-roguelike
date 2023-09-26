using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable object representing a weapon held by an entity.
/// </summary>
[CreateAssetMenu(menuName = "Game Data/Weapon")]
public class Weapon : ScriptableObject
{
    [SerializeField]
    private bool mirrorXDirection;
    public bool MirrorXDirection => mirrorXDirection;
}
