using System;
using UnityEngine;

/// <summary>
/// POCO for storing the stats for an attack. Should be the same for every instance of the attack;
/// </summary>
[Serializable]
public class AttackStats
{
    public Animation animation;
    public float damage = 1;
    public float range = 0;
    public float hitboxDuration = 1;
    public float hitStunMultiplier = 1;
    public float knockbackMultiplier = 1;
    public float startupTime = 0;
}
