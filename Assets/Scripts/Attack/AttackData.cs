using System;
using UnityEngine;

/// <summary>
/// POCO for storing data for an Attack.
/// </summary>
[Serializable]
public class AttackData
{
    public Animation animation;
    public float damage = 1;
    public float range = 1;
    public float duration = 1;

    public GameObject User { get; set; }
}
