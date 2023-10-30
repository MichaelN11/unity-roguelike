using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable object representing an item.
/// </summary>
public class Item : ScriptableObject
{
    [SerializeField]
    private ActiveAbility activeAbility;
    public ActiveAbility ActiveAbility => activeAbility;
}
