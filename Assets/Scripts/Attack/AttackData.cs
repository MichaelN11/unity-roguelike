﻿using System;
using UnityEngine;

/// <summary>
/// POCO for storing data for the instance of an Attack.
/// </summary>
[Serializable]
public class AttackData
{
    public AttackStats attackStats;
    public bool setDirectionOnHit = true;

    public GameObject User { get; set; }
    public Vector2 Direction { get; set; } = Vector2.zero;
}
