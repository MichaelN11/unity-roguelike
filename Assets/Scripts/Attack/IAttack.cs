using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface representing an attack.
/// </summary>
public interface IAttack
{
    public AttackType AttackType { get; }
    public event Action<IAttack> OnAttackUsed;

    /// <summary>
    /// Use the attack.
    /// </summary>
    /// <param name="direction">The direction of the attack</param>
    /// <param name="distance">The distance away the attack is used</param>
    /// <param name="entityType">The user's EntityType</param>
    public void Use(Vector2 direction, float distance, EntityType entityType);

    /// <summary>
    /// Interrupts the attack, preventing it from starting if it hasn't already started.
    /// </summary>
    public void Interrupt();
}
