using System;
using UnityEngine;

/// <summary>
/// Class storing data about an item that can be dropped by an entity.
/// </summary>
[Serializable]
public class ItemDrop
{
    [SerializeField]
    private Item item;
    public Item Item => item;

    [SerializeField]
    private float dropChance;
    public float DropChance => dropChance;

    [SerializeField]
    private int amount = 1;
    public int Amount => amount;
}
