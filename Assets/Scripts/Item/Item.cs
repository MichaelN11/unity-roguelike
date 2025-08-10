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

    /// <summary>
    /// Prefab held during use animation. If null, nothing is held.
    /// </summary>
    [SerializeField]
    private GameObject heldPrefab;
    public GameObject HeldPrefab => heldPrefab;

    /// <summary>
    /// Prefab used when item is dropped on the ground. If null, defaults to standard Item prefab.
    /// </summary>
    [SerializeField]
    private GameObject dropPrefab;
    public GameObject DropPrefab => dropPrefab;

    [SerializeField]
    private Sprite ingameSprite;
    public Sprite IngameSprite => ingameSprite;

    [SerializeField]
    private string itemName;
    public string ItemName => itemName;

    [SerializeField]
    private bool useOnPickup;
    public bool UseOnPickup => useOnPickup;

    [SerializeField]
    private bool animateOnUse = true;
    public bool AnimateOnUse => animateOnUse;

    [SerializeField]
    private float durationOnGround = 0;
    public float DurationOnGround => durationOnGround;

    [SerializeField]
    private Sound pickupSound;
    public Sound PickupSound => pickupSound;
}
