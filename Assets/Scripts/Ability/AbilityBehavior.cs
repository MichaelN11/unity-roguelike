using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract class representing an ability's behavior. Tied to a corresponding Ability object.
/// </summary>
public abstract class AbilityBehavior
{
    public abstract Ability Ability { get; }

    /// <summary>
    /// For the AI.
    /// TODO The AI will need more data than this. Should make an object(s).
    /// </summary>
    public abstract float Range { get; }

    protected abstract float CastTime { get; }

    protected IEnumerator coroutine;

    /// <summary>
    /// Use the ability.
    /// </summary>
    /// <param name="abilityUse">AbilityUse object containing data about how the ability was used</param>
    /// <returns>true if the ability was started successfully</returns>
    public virtual bool Use(AbilityUse abilityUse)
    {
        bool successful = false;
        if (IsUsable(abilityUse))
        {
            OnUse(abilityUse);
            coroutine = Delay(abilityUse);
            abilityUse.Component.StartCoroutine(coroutine);
            successful = true;
        }
        return successful;
    }

    /// <summary>
    /// Interrupts the ability, preventing it from starting if it hasn't already started.
    /// </summary>
    public virtual void Interrupt(AbilityManager component)
    {
        if (coroutine != null)
        {
            component.StopCoroutine(coroutine);
        }
    }

    /// <summary>
    /// Determines if the ability is usable in the current state.
    /// </summary>
    /// <param name="abilityUse">AbilityUse object containing data about how the ability was used</param>
    public virtual bool IsUsable(AbilityUse abilityUse)
    {
        return abilityUse.UserState.CanAct();
    }

    /// <summary>
    /// Method called when the ability is successfully used. This is called before the initial cast time.
    /// </summary>
    /// <param name="abilityUse">AbilityUse object containing data about how the ability was used</param>
    protected virtual void OnUse(AbilityUse abilityUse)
    {
        abilityUse.User.LookDirection = abilityUse.Direction;
        abilityUse.UserState.AbilityState(CastTime);
    }

    /// <summary>
    /// Starts the ability. This is called after the initial cast time.
    /// </summary>
    /// <param name="abilityUse">AbilityUse object containing data about how the ability was used</param>
    protected abstract void StartAbility(AbilityUse abilityUse);

    /// <summary>
    /// Coroutine method that delays the ability's start time. Used for cast or startup times.
    /// </summary>
    /// <param name="abilityUse">AbilityUse object containing data about how the ability was used</param>
    /// <returns>IEnumerator used for the coroutine</returns>
    protected virtual IEnumerator Delay(AbilityUse abilityUse)
    {
        yield return new WaitForSeconds(CastTime);
        StartAbility(abilityUse);
    }
}
