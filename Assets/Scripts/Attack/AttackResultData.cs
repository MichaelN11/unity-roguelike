using System;
using UnityEngine;

/// <summary>
/// POCO containing data for the results of an attack.
/// </summary>
public class AttackResultData
{
    public bool IsDead { get; set; } = false;
    public float HitStunDuration { get; set; } = 0;
    public float KnockbackSpeed { get; set; } = 0;
    public float KnockbackAcceleration { get; set; } = 0;
    public Vector2 KnockbackDirection { get; set; } = Vector2.zero;
}
