using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// POCO containing attack data for an effect.
/// </summary>
[Serializable]
public class AttackEffectData
{
    [SerializeField]
    private Sound soundOnHit;
    public Sound SoundOnHit => soundOnHit;

    [SerializeField]
    private float damage = 1;
    public float Damage => damage;

    [SerializeField]
    private float attackDistance = 0;
    public float AttackDistance => attackDistance;

    /// <summary>
    /// If this value is greater than the target's poise, the target will be stunned.
    /// </summary>
    [SerializeField]
    private float stunPower = 1;
    public float StunPower => stunPower;

    [SerializeField]
    private float hitStunMultiplier = 1;
    public float HitStunMultiplier => hitStunMultiplier;

    [SerializeField]
    private float knockbackMultiplier = 1;
    public float KnockbackMultiplier => knockbackMultiplier;

    [SerializeField]
    private float hitStop = 0.06f;
    public float HitStop => hitStop;

    [SerializeField]
    private AttackDescription description;
    public AttackDescription Description => description;

    [SerializeField]
    private float damageIncreaseFromCharge = 0;
    public float DamageIncreaseFromCharge => damageIncreaseFromCharge;

    [SerializeField]
    private float hitStunIncreaseFromCharge = 0;
    public float HitStunIncreaseFromCharge => hitStunIncreaseFromCharge;

    [SerializeField]
    private float knockbackIncreaseFromCharge = 0;
    public float KnockbackIncreaseFromCharge => knockbackIncreaseFromCharge;
}
