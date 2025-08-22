using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// An ability that can be charged up.
/// </summary>
[CreateAssetMenu(menuName = "Game Data/Ability/Chargeable")]
public class ChargeableAbility : ActiveAbility
{
    [SerializeField]
    private CommonAbilityData abilityData = new();
    public CommonAbilityData AbilityData => abilityData;

    /// <summary>
    /// The amount of seconds an ability can be charged to increase its effects.
    /// </summary>
    [SerializeField]
    private float chargeableTime = 0;
    public float ChargeableTime => chargeableTime;

    public override AbilityUseEventInfo Use(Vector2 direction, float offsetDistance, AbilityUseData abilityUse, EntityAbilityContext entityAbilityContext)
    {
        if (abilityUse.EntityState.CanAct() || (abilityData.CanCancelInto && AbilityUtil.IsReadyToCancel(abilityUse, entityAbilityContext, this)))
        {
            if (!entityAbilityContext.IsAbilityCharging)
            {
                AbilityUtil.SetCurrentAbility(this, abilityUse, direction, entityAbilityContext);
                abilityUse.EntityState.UseAbility();
                entityAbilityContext.IsAbilityCharging = true;
                AbilityUseEventInfo abilityUseEvent = AbilityUtil.BuildAbilityUseEventInfo(abilityUse, abilityData);
                return abilityUseEvent;
            }
        }
        return null;
    }

    /// <summary>
    /// The chargeable ability is released.
    /// </summary>
    /// <returns>true if the ability was stopped by the release</returns>
    public override bool Release(Vector2 direction, float offsetDistance, AbilityUseData abilityUse, EntityAbilityContext entityAbilityContext)
    {
        if (entityAbilityContext.IsAbilityCharging
            && entityAbilityContext.CurrentActiveAbility == this)
        {
            if (abilityUse.Movement != null)
            {
                abilityUse.Movement.StopMoving();
            }

            if (entityAbilityContext.ChargeTimer > abilityData.CastTime)
            {
                abilityUse.EntityState.HardcastingState(abilityData.RecoveryTime + abilityData.ActiveAnimationTime, abilityData.AimWhileCasting);
                abilityUse.ChargePercent = Math.Min(1, entityAbilityContext.ChargeTimer / ChargeableTime);
                Activate(abilityUse, offsetDistance, entityAbilityContext);
            }
            else
            {
                float timeRemainingToCast = abilityData.CastTime - entityAbilityContext.ChargeTimer;
                abilityUse.EntityState.UseAbility(timeRemainingToCast);
                entityAbilityContext.DelayedAbilityCoroutine = DelayAbility(abilityUse, offsetDistance, timeRemainingToCast, entityAbilityContext);
                abilityUse.AbilityManager.StartCoroutine(entityAbilityContext.DelayedAbilityCoroutine);
            }
            entityAbilityContext.IsAbilityCharging = false;
            entityAbilityContext.ChargeTimer = 0;
            return true;
        }

        return false;
    }

    public override void Interrupt(AbilityUseData abilityUse, float currentDuration, EntityAbilityContext entityAbilityContext)
    {
        if (entityAbilityContext.CurrentAbilityStarted)
        {
            if (abilityData.StopSoundAfterUse)
            {
                AudioManager.Instance.StopSound(abilityData.SoundOnUse);
            }
            AbilityUtil.InterruptEffects(abilityData.Effects, abilityUse, abilityData.Duration, currentDuration);
        }
        entityAbilityContext.IsAbilityCharging = false;
        entityAbilityContext.ChargeTimer = 0;
    }

    /// <summary>
    /// Coroutine method that delays the charged on use ability's start time. Sets the state for the active and recovery frames.
    /// </summary>
    /// <returns>IEnumerator used for the coroutine</returns>
    private IEnumerator DelayAbility(AbilityUseData abilityUse, float offsetDistance, float castTime, EntityAbilityContext entityAbilityContext)
    {
        yield return new WaitForSeconds(castTime);
        Activate(abilityUse, offsetDistance, entityAbilityContext);
        abilityUse.EntityState.HardcastingState(abilityData.RecoveryTime + abilityData.ActiveAnimationTime, abilityData.AimWhileCasting);
    }

    private void Activate(AbilityUseData abilityUse, float offsetDistance, EntityAbilityContext entityAbilityContext)
    {
        AbilityUtil.UpdateAbilityState(abilityUse, offsetDistance, entityAbilityContext);
        AbilityUtil.PlayActivationSounds(abilityData, abilityUse);
        AbilityUtil.ActivateEffects(abilityData.Effects, abilityUse, abilityData.Duration);

        AbilityUseEventInfo abilityUseEvent = AbilityUtil.BuildAbilityUseEventInfo(abilityUse, abilityData);
        abilityUseEvent.Origin = entityAbilityContext.CurrentAbilityOrigin;
        abilityUse.AbilityManager.InvokeAbilityUseEvent(abilityUseEvent);
    }
}
