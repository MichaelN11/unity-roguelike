using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// An ability that causes effects immediately when it is used.
/// </summary>
[CreateAssetMenu(menuName = "Game Data/Ability/On Use")]
public class OnUseAbility : ActiveAbility
{
    [SerializeField]
    private List<AbilityEffect> effects;
    public List<AbilityEffect> Effects => effects;

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

    [SerializeField]
    private AbilityAnimation abilityAnimation;
    public AbilityAnimation AbilityAnimation => abilityAnimation;

    /// <summary>
    /// Indicates that the ability does not depend on or change the entity's state.
    /// </summary>
    [SerializeField]
    private bool statelessCast = false;
    public bool StatelessCast => statelessCast;

    /// <summary>
    /// The amount of seconds an ability can be charged to increase its effects. 0 indicates the ability is not chargeable.
    /// </summary>
    [SerializeField]
    private float chargeableTime = 0;
    public float ChargeableTime => chargeableTime;

    public void Use(AbilityUseData abilityUse)
    {
        if (soundOnUse != null)
        {
            AudioManager.Instance.Play(soundOnUse);
            if (stopSoundAfterUse)
            {
                IEnumerator soundLoopCoroutine = StopLoopedSound();
                abilityUse.AbilityManager.StartCoroutine(soundLoopCoroutine);
            }
        }
        foreach (AbilityEffect abilityEffect in effects) {
            if (abilityEffect.SoundOnUse != null)
            {
                AudioManager.Instance.Play(abilityEffect.SoundOnUse);
            }
            EffectUseData effectUseData = new();
            // Adding the EffectUseData into the list with the same order as the effects are stored in the ability
            abilityUse.EffectUseDataList.Add(effectUseData);
            abilityEffect.Trigger(abilityUse, effectUseData);

            float effectDuration = GetEffectDuration(abilityEffect);
            if (effectDuration > 0)
            {
                IEnumerator coroutine = EndEffect(abilityEffect, abilityUse, effectDuration, effectUseData);
                abilityUse.AbilityManager.StartCoroutine(coroutine);
            }
        }
    }

    public void Interrupt(AbilityUseData abilityUse, float currentDuration)
    {
        if (stopSoundAfterUse)
        {
            AudioManager.Instance.StopSound(soundOnUse);
        }
        int index = 0;
        foreach (AbilityEffect abilityEffect in effects)
        {
            if (GetEffectDuration(abilityEffect) > currentDuration)
            {
                // Retrieve the EffectUseData using the index of the effect within the ability's effect list
                abilityEffect.Unapply(abilityUse, abilityUse.EffectUseDataList[index]);
            }
            index++;
        }
    }

    private IEnumerator EndEffect(AbilityEffect effect, AbilityUseData effectData, float effectDuration, EffectUseData effectUseData)
    {
        yield return new WaitForSeconds(effectDuration);
        effect.Unapply(effectData, effectUseData);
    }

    private float GetEffectDuration(AbilityEffect abilityEffect)
    {
        return (abilityEffect.UseAbilityDuration) ? duration : abilityEffect.Duration;
    }

    private IEnumerator StopLoopedSound()
    {
        yield return new WaitForSeconds(activeAnimationTime);
        AudioManager.Instance.StopSound(soundOnUse);
    }
}
