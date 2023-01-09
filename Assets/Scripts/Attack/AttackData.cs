using System;
using UnityEngine;

/// <summary>
/// POCO for storing data for the instance of an Attack.
/// </summary>
[Serializable]
public class AttackData
{
    public AttackStats attackStats;
    public GameObject user;
    public Vector2 direction = Vector2.zero;
    public bool setDirectionOnHit = true;
}
