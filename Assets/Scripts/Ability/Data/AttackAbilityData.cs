using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// POCO containing attack data for an ability.
/// </summary>
[Serializable]
public class AttackAbilityData
{
    [SerializeField]
    private Sound soundOnUse;
    public Sound SoundOnUse => soundOnUse;

    [SerializeField]
    private Sound soundOnHit;
    public Sound SoundOnHit => soundOnHit;

    [SerializeField]
    private float damage = 1;
    public float Damage => damage;

    [SerializeField]
    private float attackDistance = 0;
    public float AttackDistance => attackDistance;

    [SerializeField]
    private float radius = 0;
    public float Radius => radius;

    [SerializeField]
    private float hitStunMultiplier = 1;
    public float HitStunMultiplier => hitStunMultiplier;

    [SerializeField]
    private float knockbackMultiplier = 1;
    public float KnockbackMultiplier => knockbackMultiplier;

    [SerializeField]
    private float recoveryDuration = 1f;
    public float RecoveryDuration => recoveryDuration;

    [SerializeField]
    private float hitStop = 0.06f;
    public float HitStop => hitStop;
}
