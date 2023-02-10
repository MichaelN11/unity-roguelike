﻿using System;

/// <summary>
/// Container for events related to an attack.
/// </summary>
public class AttackEvents
{
    public event Action<AttackData> OnAttackUsed;
    public event Action<AttackData> OnAttackSuccessful;

    /// <summary>
    /// Invokes the attack used event.
    /// </summary>
    /// <param name="attackData">The attack data for the event</param>
    public void invokeAttackUsed(AttackData attackData)
    {
        OnAttackUsed?.Invoke(attackData);
    }

    /// <summary>
    /// Invokes the attack successful event.
    /// </summary>
    /// <param name="attackData">The attack data for the event</param>
    public void invokeAttackSuccessful(AttackData attackData)
    {
        OnAttackSuccessful?.Invoke(attackData);
    }
}