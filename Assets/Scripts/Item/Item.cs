using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable object representing an item.
/// </summary>
[CreateAssetMenu(menuName = "Game Data/Item")]
public class Item : ScriptableObject
{
    [SerializeField]
    private ActiveAbility activeAbility;
    public ActiveAbility ActiveAbility => activeAbility;

    [SerializeField]
    private GameObject prefab;
    public GameObject Prefab => prefab;
}
