using System;
using UnityEngine;

/// <summary>
/// POCO for storing data for the instance of an Attack.
/// </summary>
public class AttackData
{
    public AttackStats AttackStats { get; set; }
    public GameObject User { get; set; }
    public Vector2 Direction { get; set; }
}
