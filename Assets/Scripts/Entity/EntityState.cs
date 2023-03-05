using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component controlling the entity's state.
/// </summary>
public class EntityState : MonoBehaviour
{
    public event Action UnstunnedEvent;

    public Vector2 LookDirection { get; set; } = Vector2.zero;
    public ActionState ActionState { get; private set; } = ActionState.Stand;
    public float StunTimer { get; private set; } = 0f;
    public float FlashTimer { get; private set; } = 0f;
    public float StopTimer { get; private set; } = 0f;

    private AbilityManager abilityManager;

    private void Awake()
    {
        abilityManager = GetComponentInChildren<AbilityManager>();
    }

    private void Update()
    {
        if (IsStopped())
        {
            UpdateStopTimer();
        }
        else
        {
            UpdateStunTimer();
            UpdateFlashTimer();
        }
    }

    /// <summary>
    /// Makes the entity stop for the passed duration. Used for hit stop.
    /// </summary>
    /// <param name="duration">The stop duration as a float</param>
    public void Stop(float duration)
    {
        StopTimer = duration;
    }

    /// <summary>
    /// Makes the entity flash for the passed duration. Used when entity takes damage.
    /// </summary>
    /// <param name="duration">The flash duration as a float</param>
    public void Flash(float duration)
    {
        FlashTimer = duration;
    }

    /// <summary>
    /// Determines if the entity is flashing.
    /// </summary>
    /// <returns>true if the entity is flashing</returns>
    public bool IsFlashing()
    {
        return FlashTimer > 0;
    }

    /// <summary>
    /// Determines if the entity is stopped/frozen from something like hit stop.
    /// </summary>
    /// <returns>true if the entity is stopped</returns>
    public bool IsStopped()
    {
        return StopTimer > 0;
    }

    /// <summary>
    /// Determines if the entity is stunned.
    /// </summary>
    /// <returns>true if the entity is stunned</returns>
    public bool IsStunned()
    {
        return ActionState == ActionState.Ability
            || ActionState == ActionState.Hitstun;
    }

    /// <summary>
    /// Determines if the entity is able to act.
    /// </summary>
    /// <returns>true if the entity can act</returns>
    public bool CanAct()
    {
        return !IsStunned()
            && ActionState != ActionState.Dead;
    }

    /// <summary>
    /// Changes state to the Ability state, using the passed duration.
    /// </summary>
    /// <param name="duration">The time in the ability state as a float</param>
    public void AbilityState(float duration)
    {
        ActionState = ActionState.Ability;
        StunTimer = duration;
    }

    /// <summary>
    /// Changes state to the Hitstun state, using the passed duration.
    /// </summary>
    /// <param name="duration">The time in the ability state as a float</param>
    public void HitstunState(float duration)
    {
        ActionState = ActionState.Hitstun;
        StunTimer = duration;
        if (abilityManager != null)
        {
            abilityManager.Interrupt();
        }
    }

    /// <summary>
    /// Changes state to Dead.
    /// </summary>
    public void DeadState()
    {
        ActionState = ActionState.Dead;
        if (abilityManager != null)
        {
            abilityManager.Interrupt();
        }
    }

    /// <summary>
    /// Changes state to Move.
    /// </summary>
    public void MoveState()
    {
        ActionState = ActionState.Move;
    }

    /// <summary>
    /// Changes state to Stand.
    /// </summary>
    public void StandState()
    {
        ActionState = ActionState.Stand;
    }

    /// <summary>
    /// Changes state to Idle.
    /// </summary>
    public void IdleState()
    {
        ActionState = ActionState.Idle;
    }

    /// <summary>
    /// If the entity is stunned/attacking, subtract the time in seconds passed from the last
    /// update. If the timer is below 0, tell the entity to change state. Also sets
    /// the move direction to any move direction attempted while the entity was stunned.
    /// </summary>
    private void UpdateStunTimer()
    {
        if (IsStunned())
        {
            StunTimer -= Time.deltaTime;
            if (StunTimer <= 0)
            {
                StandState();
                UnstunnedEvent?.Invoke();
            }
        }
    }

    /// <summary>
    /// Updates the flash timer, which controls how long the entity is flashing white.
    /// </summary>
    private void UpdateFlashTimer()
    {
        if (IsFlashing())
        {
            FlashTimer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Updates the stop timer, which controls how long the entity is stopped (paused).
    /// </summary>
    private void UpdateStopTimer()
    {
        StopTimer -= Time.deltaTime;
    }
}
