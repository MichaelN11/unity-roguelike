using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component for handling damage calculations and health.
/// </summary>
public class Damageable : MonoBehaviour
{
    public float maxHealth = 1;
    [SerializeField]
    private float hitStunDuration = 1;
    [SerializeField]
    private float knockbackSpeed = 1;
    [SerializeField]
    private float knockbackAcceleration = 0;

    public float CurrentHealth { get; private set; }

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    /// <summary>
    /// Handles an incoming attack using the passed AttackData. Returns the HitStun information.
    /// </summary>
    /// <param name="attackData">The stats and data for the attack</param>
    /// <returns>A HitStun object</returns>
    public AttackResult HandleAttack(AttackData attackData)
    {
        TakeDamage(attackData.AttackType.Damage);

        AttackResult attackResult = new();
        attackResult.IsDead = IsDead();
        attackResult.HitStunDuration = hitStunDuration * attackData.AttackType.HitStunMultiplier;
        attackResult.KnockbackSpeed = knockbackSpeed * attackData.AttackType.KnockbackMultiplier;
        attackResult.KnockbackDirection = attackData.Direction;
        attackResult.KnockbackAcceleration = knockbackAcceleration;
        return attackResult;
    }

    /// <summary>
    /// Take damage to health.
    /// </summary>
    /// <param name="damage">The damage being dealt</param>
    private void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
    }

    /// <summary>
    /// Determines if the entity is dead.
    /// </summary>
    /// <returns>true if the entity is dead</returns>
    private bool IsDead()
    {
        return CurrentHealth <= 0;
    }
}
