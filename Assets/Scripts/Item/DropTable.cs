using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A drop table for items in a level.
/// </summary>
[CreateAssetMenu(menuName = "Game Data/Drop Table")]
public class DropTable : ScriptableObject
{
    /// <summary>
    /// If the drop table is using weighted values as opposed to percents.
    /// </summary>
    [SerializeField]
    private bool isWeighted = true;
    public bool IsWeighted => isWeighted;

    [field: SerializeField]
    public List<ItemDrop> ItemDrops { get; set; }
}
