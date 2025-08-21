using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Utility class for shared ability methods.
/// </summary>
public class AbilityUtil
{

    /// <summary>
    /// Activates the list of ability effects. 
    /// </summary>
    /// <param name="effects">list of effects</param>
    /// <param name="abilityUse">ability use data</param>
    /// <param name="abilityDuration">The duration of the ability. Each effect can decide whether or not to use it.</param>
    public static void ActivateEffects(List<AbilityEffect> effects, AbilityUseData abilityUse, float abilityDuration)
    {
        foreach (AbilityEffect abilityEffect in effects)
        {
            if (abilityEffect.SoundOnUse != null)
            {
                AudioManager.Instance.Play(abilityEffect.SoundOnUse);
            }
            EffectUseData effectUseData = new();
            // Adding the EffectUseData into the list with the same order as the effects are stored in the ability
            abilityUse.EffectUseDataList.Add(effectUseData);
            abilityEffect.Trigger(abilityUse, effectUseData);

            float effectDuration = GetEffectDuration(abilityEffect, abilityDuration);
            if (effectDuration > 0)
            {
                IEnumerator coroutine = EndEffect(abilityEffect, abilityUse, effectDuration, effectUseData);
                abilityUse.AbilityManager.StartCoroutine(coroutine);
            }
        }
    }

    public static void InterruptEffects(List<AbilityEffect> effects, AbilityUseData abilityUse, float abilityDuration, float currentDuration)
    {
        int index = 0;
        foreach (AbilityEffect abilityEffect in effects)
        {
            if (GetEffectDuration(abilityEffect, abilityDuration) > currentDuration)
            {
                // Retrieve the EffectUseData using the index of the effect within the ability's effect list
                abilityEffect.Unapply(abilityUse, abilityUse.EffectUseDataList[index]);
            }
            index++;
        }
    }

    public static void SetCurrentAbility(ActiveAbility activeAbility, AbilityUseData abilityUse,
        Vector2 direction, EntityAbilityContext entityAbilityContext)
    {
        abilityUse.AbilityManager.InterruptCurrentAbility();
        entityAbilityContext.CurrentActiveAbility = activeAbility;
        entityAbilityContext.CurrentAbilityData = abilityUse;
        abilityUse.EntityState.LookDirection = direction;
    }

    public static AbilityUseEventInfo BuildAbilityUseEventInfo(AbilityUseData abilityUse, CommonAbilityData abilityData)
    {
        AbilityUseEventInfo abilityUseEvent = new()
        {
            AbilityUse = abilityUse,
            AbilityAnimation = abilityData.AbilityAnimation,
            CastTime = abilityData.CastTime,
            ActiveTime = abilityData.ActiveAnimationTime,
            RecoveryTime = abilityData.RecoveryTime,
            AimDuration = abilityData.AimDuration,
            Range = abilityData.Range,
            ChangeDirection = abilityData.ChangeDirection
        };
        return abilityUseEvent;
    }

    public static bool IsReadyToCancel(AbilityUseData abilityUse, EntityAbilityContext entityAbilityContext, ActiveAbility activeAbility)
    {
        return abilityUse.EntityState.ActionState == ActionState.Hardcasting
            && abilityUse.EntityState.StunTimer <= entityAbilityContext.PreviousCancelableDuration
            && (entityAbilityContext.CurrentActiveAbility == null || entityAbilityContext.CurrentActiveAbility != activeAbility);
    }

    public static void PlayActivationSounds(CommonAbilityData abilityData, AbilityUseData abilityUse)
    {
        if (abilityData.SoundOnUse != null)
        {
            AudioManager.Instance.Play(abilityData.SoundOnUse);
            if (abilityData.StopSoundAfterUse)
            {
                IEnumerator soundLoopCoroutine = StopLoopedSound(abilityData);
                abilityUse.AbilityManager.StartCoroutine(soundLoopCoroutine);
            }
        }
    }

    // TODO what and why is this method???
    public static void UpdateAbilityState(AbilityUseData abilityUse, float offsetDistance, EntityAbilityContext entityAbilityContext)
    {
        abilityUse.EntityState.CanLookWhileCasting = false;
        abilityUse.Direction = abilityUse.EntityState.LookDirection.normalized;
        abilityUse.Position += abilityUse.EntityState.LookDirection.normalized * offsetDistance;
        entityAbilityContext.CurrentAbilityStarted = true;
        entityAbilityContext.CurrentAbilityDuration = 0;
    }

    private static IEnumerator EndEffect(AbilityEffect effect, AbilityUseData effectData, float effectDuration, EffectUseData effectUseData)
    {
        yield return new WaitForSeconds(effectDuration);
        effect.Unapply(effectData, effectUseData);
    }

    private static float GetEffectDuration(AbilityEffect abilityEffect, float abilityDuration)
    {
        return (abilityEffect.UseAbilityDuration) ? abilityDuration : abilityEffect.Duration;
    }

    private static IEnumerator StopLoopedSound(CommonAbilityData abilityData)
    {
        yield return new WaitForSeconds(abilityData.ActiveAnimationTime);
        AudioManager.Instance.StopSound(abilityData.SoundOnUse);
    }
}
