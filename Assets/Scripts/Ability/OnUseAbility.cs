using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An ability that causes effects immediately when it is used.
/// </summary>
[CreateAssetMenu(menuName = "Game Data/Ability/On Use")]
public class OnUseAbility : Ability
{
    [SerializeField]
    private List<AbilityEffect> effects;
    public List<AbilityEffect> Effects => effects;

    [SerializeField]
    private float castTime;
    public float CastTime => castTime;

    [SerializeField]
    private float recoveryTime;
    public float RecoveryTime => recoveryTime;

    [SerializeField]
    private float activeTime;
    public float ActiveTime => activeTime;

    [SerializeField]
    private float cooldown;
    public float Cooldown => cooldown;

    [SerializeField]
    private bool canCancelInto = false;
    public bool CanCancelInto => canCancelInto;

    [SerializeField]
    private float cancelableDuration = 0;
    public float CancelableDuration => cancelableDuration;

    /// <summary>
    /// TODO Need a better way for AI to determine effects.
    /// </summary>
    [SerializeField]
    private float aiRange;
    public float AIRange => aiRange;

    /// <summary>
    /// Time after using the ability to continue facing in the direction the entity is aiming in.
    /// </summary>
    [SerializeField]
    private float aimDuration = 0;
    public float AimDuration => aimDuration;

    [SerializeField]
    private Sound soundOnUse;
    public Sound SoundOnUse => soundOnUse;

    [SerializeField]
    private AbilityAnimation abilityAnimation;
    public AbilityAnimation AbilityAnimation => abilityAnimation;

    public void Use(EffectData abilityUse)
    {
        AudioManager.Instance.Play(soundOnUse);
        foreach (AbilityEffect abilityEffect in effects) {
            if (abilityEffect.SoundOnUse != null)
            {
                AudioManager.Instance.Play(abilityEffect.SoundOnUse);
            }
            abilityEffect.Trigger(abilityUse);
        }
    }

    public void Interrupt(EffectData abilityUse)
    {
        foreach (AbilityEffect abilityEffect in effects)
        {
            abilityEffect.Interrupt(abilityUse);
        }
    }
}
