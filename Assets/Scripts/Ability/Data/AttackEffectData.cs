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

    [SerializeField]
    private float hitStunMultiplier = 1;
    public float HitStunMultiplier => hitStunMultiplier;

    [SerializeField]
    private float knockbackMultiplier = 1;
    public float KnockbackMultiplier => knockbackMultiplier;

    [SerializeField]
    private float hitStop = 0.06f;
    public float HitStop => hitStop;
}
