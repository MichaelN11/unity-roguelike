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

    public override AbilityUseEventInfo Use(Vector2 direction, float offsetDistance, AbilityUseData abilityUse, EntityAbilityContext entityAbilityContext)
    {
        if (abilityUse.EntityState.CanAct() || (canCancelInto && AbilityUtil.IsReadyToCancel(abilityUse, entityAbilityContext, this)))
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
        if (SoundOnCast != null)
        {
            AudioManager.Instance.Play(SoundOnCast);
        }
        if (abilityUse.Movement != null)
        {
            abilityUse.Movement.StopMoving();
        }
        AbilityUtil.SetCurrentAbility(this, abilityUse, direction, entityAbilityContext);
        abilityUse.EntityState.HardcastingState(RecoveryTime + CastTime + ActiveAnimationTime, AimWhileCasting);
        AbilityUseEventInfo abilityUseEvent = BuildAbilityUseEventInfo(abilityUse);
        abilityUse.AbilityManager.InvokeAbilityStartedEvent(abilityUseEvent);
        entityAbilityContext.PreviousCancelableDuration = CancelableDuration;

        return abilityUseEvent;
    }

    public void Activate(AbilityUseData abilityUse)
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
        AbilityUtil.ActivateEffects(effects, abilityUse, duration);
    }

    // TODO replace with util method
    public AbilityUseEventInfo BuildAbilityUseEventInfo(AbilityUseData abilityUse)
    {
        AbilityUseEventInfo abilityUseEvent = new()
        {
            AbilityUse = abilityUse,
            AbilityAnimation = AbilityAnimation,
            CastTime = CastTime,
            ActiveTime = ActiveAnimationTime,
            RecoveryTime = RecoveryTime,
            AimDuration = AimDuration,
            Range = Range,
            ChangeDirection = ChangeDirection
        };
        return abilityUseEvent;
    }

    public override UsableAbilityInfo GetUsableAbilityInfo(EntityAbilityContext entityAbilityContext)
    {
        return new UsableAbilityInfo()
        {
            Range = Range
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
        float castTime = (castTimeOverride >= 0) ? castTimeOverride : CastTime;
        yield return new WaitForSeconds(castTime);
        StartActivatingAbility(abilityUse, offsetDistance, entityAbilityContext);
    }

    private void StartActivatingAbility(AbilityUseData abilityUse, float offsetDistance, EntityAbilityContext entityAbilityContext)
    {
        AbilityUtil.UpdateAbilityState(abilityUse, offsetDistance, entityAbilityContext);
        Activate(abilityUse);

        AbilityUseEventInfo abilityUseEvent = BuildAbilityUseEventInfo(abilityUse);
        abilityUseEvent.Origin = entityAbilityContext.CurrentAbilityOrigin;
        abilityUse.AbilityManager.InvokeAbilityUseEvent(abilityUseEvent);
    }

    public override void Interrupt(AbilityUseData abilityUse, float currentDuration, EntityAbilityContext entityAbilityContext)
    {
        if (entityAbilityContext.CurrentAbilityStarted)
        {
            if (stopSoundAfterUse)
            {
                AudioManager.Instance.StopSound(soundOnUse);
            }
            AbilityUtil.InterruptEffects(effects, abilityUse, duration, currentDuration);
        }
    }

    private IEnumerator StopLoopedSound()
    {
        yield return new WaitForSeconds(activeAnimationTime);
        AudioManager.Instance.StopSound(soundOnUse);
    }
}
