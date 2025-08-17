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

    /// <summary>
    /// Which animations to display the weapon during. Empty list defaults to all animations.
    /// </summary>
    [SerializeField]
    private List<AbilityAnimation> displayAnimations;
    public List<AbilityAnimation> DisplayAnimations => displayAnimations;

    /// <summary>
    /// Animations that will swing the weapon by rotating it.
    /// </summary>
    [SerializeField]
    private List<AbilityAnimation> swingRotateAnimations;
    public List<AbilityAnimation> SwingRotateAnimations => swingRotateAnimations;
}
