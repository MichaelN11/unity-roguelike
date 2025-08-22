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
    private CommonAbilityData abilityData = new();
    public CommonAbilityData AbilityData => abilityData;

    public override AbilityUseEventInfo Use(Vector2 direction, float offsetDistance, AbilityUseData abilityUse, EntityAbilityContext entityAbilityContext)
    {
        if (abilityUse.EntityState.CanAct() || (abilityData.CanCancelInto && AbilityUtil.IsReadyToCancel(abilityUse, entityAbilityContext, this)))
        {
            AbilityUseEventInfo abilityUseEvent = StartCastingAbility(direction, abilityUse, entityAbilityContext);
            entityAbilityContext.DelayedAbilityCoroutine = DelayUse(abilityUseEvent.AbilityUse, offsetDistance, entityAbilityContext);
            abilityUse.AbilityManager.StartCoroutine(entityAbilityContext.DelayedAbilityCoroutine);
            return abilityUseEvent;
        }
        return null;
    }

    public AbilityUseEventInfo StartCastingAbility(Vector2 direction, AbilityUseData abilityUse, EntityAbilityContext entityAbilityContext)
    {
        if (abilityData.SoundOnCast != null)
        {
            AudioManager.Instance.Play(abilityData.SoundOnCast);
        }
        if (abilityUse.Movement != null)
        {
            abilityUse.Movement.StopMoving();
        }
        AbilityUtil.SetCurrentAbility(this, abilityUse, direction, entityAbilityContext);
        abilityUse.EntityState.HardcastingState(abilityData.RecoveryTime + abilityData.CastTime
            + abilityData.ActiveAnimationTime, abilityData.AimWhileCasting);
        AbilityUseEventInfo abilityUseEvent = AbilityUtil.BuildAbilityUseEventInfo(abilityUse, abilityData);
        abilityUse.AbilityManager.InvokeAbilityStartedEvent(abilityUseEvent);
        entityAbilityContext.PreviousCancelableDuration = abilityData.CancelableDuration;

        return abilityUseEvent;
    }

    public void Activate(AbilityUseData abilityUse)
    {
        if (abilityData.SoundOnUse != null)
        {
            AudioManager.Instance.Play(abilityData.SoundOnUse);
            if (abilityData.StopSoundAfterUse)
            {
                IEnumerator soundLoopCoroutine = StopLoopedSound();
                abilityUse.AbilityManager.StartCoroutine(soundLoopCoroutine);
            }
        }
        AbilityUtil.ActivateEffects(abilityData.Effects, abilityUse, abilityData.Duration);
    }

    public override UsableAbilityInfo GetUsableAbilityInfo(EntityAbilityContext entityAbilityContext)
    {
        return new UsableAbilityInfo()
        {
            Range = abilityData.Range
        };
    }

    /// <summary>
    /// Coroutine method that delays the on use ability's start time. Used for cast or startup times.
    /// </summary>
    /// <param name="abilityUse"></param>
    /// <param name="offsetDistance"></param>
    /// <returns>IEnumerator used for the coroutine</returns>
    private IEnumerator DelayUse(AbilityUseData abilityUse, float offsetDistance, EntityAbilityContext entityAbilityContext, float castTimeOverride = -1)
    {
        float castTime = (castTimeOverride >= 0) ? castTimeOverride : abilityData.CastTime;
        yield return new WaitForSeconds(castTime);
        StartActivatingAbility(abilityUse, offsetDistance, entityAbilityContext);
    }

    private void StartActivatingAbility(AbilityUseData abilityUse, float offsetDistance, EntityAbilityContext entityAbilityContext)
    {
        AbilityUtil.UpdateAbilityState(abilityUse, offsetDistance, entityAbilityContext);
        Activate(abilityUse);

        AbilityUseEventInfo abilityUseEvent = AbilityUtil.BuildAbilityUseEventInfo(abilityUse, abilityData);
        abilityUseEvent.Origin = entityAbilityContext.CurrentAbilityOrigin;
        abilityUse.AbilityManager.InvokeAbilityUseEvent(abilityUseEvent);
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
    }

    private IEnumerator StopLoopedSound()
    {
        yield return new WaitForSeconds(abilityData.ActiveAnimationTime);
        AudioManager.Instance.StopSound(abilityData.SoundOnUse);
    }
}
