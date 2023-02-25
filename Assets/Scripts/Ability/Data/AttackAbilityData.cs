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
    private AttackAnimation attackAnimation = AttackAnimation.Default;
    public AttackAnimation AttackAnimation => attackAnimation;

    [SerializeField]
    private float damage = 1;
    public float Damage => damage;

    [SerializeField]
    private float range = 0;
    public float Range => range;

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
    private float attackDuration = 1f;
    public float AttackDuration => attackDuration;

    [SerializeField]
    private float hitStop = 0.06f;
    public float HitStop => hitStop;
}
