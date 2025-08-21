using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class storing common ability fields in a serializable format. Used by ability types that are mostly standard but differ
/// in how they are activated/used.
/// </summary>
[Serializable]
public class CommonAbilityData
{
    [SerializeField]
    private List<AbilityEffect> effects;
    public List<AbilityEffect> Effects => effects;

    [SerializeField]
    private AbilityAnimation abilityAnimation;
    public AbilityAnimation AbilityAnimation => abilityAnimation;

    [SerializeField]
    private float duration;
    public float Duration => duration;

    [SerializeField]
    private float castTime;
    public float CastTime => castTime;

    [SerializeField]
    private float recoveryTime;
    public float RecoveryTime => recoveryTime;

    [SerializeField]
    private float activeAnimationTime;
    public float ActiveAnimationTime => activeAnimationTime;

    [SerializeField]
    private bool canCancelInto = false;
    public bool CanCancelInto => canCancelInto;

    [SerializeField]
    private float cancelableDuration = 0;
    public float CancelableDuration => cancelableDuration;

    /// <summary>
    /// TODO Need a better way to determine this.
    /// </summary>
    [SerializeField]
    private float range;
    public float Range => range;

    [SerializeField]
    private bool aimWhileCasting = false;
    public bool AimWhileCasting => aimWhileCasting;

    /// <summary>
    /// Time after using the ability to continue facing in the direction the entity is aiming in.
    /// </summary>
    [SerializeField]
    private float aimDuration = 0;
    public float AimDuration => aimDuration;

    [SerializeField]
    private bool changeDirection = true;
    public bool ChangeDirection => changeDirection;

    [SerializeField]
    private Sound soundOnCast;
    public Sound SoundOnCast => soundOnCast;

    [SerializeField]
    private Sound soundOnUse;
    public Sound SoundOnUse => soundOnUse;

    [SerializeField]
    private bool stopSoundAfterUse;
    public bool StopSoundAfterUse => stopSoundAfterUse;
}
