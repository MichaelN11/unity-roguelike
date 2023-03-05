using System;

/// <summary>
/// Container for events related to a specific attack instance.
/// </summary>
public class AttackEvents
{
    /// <summary>
    /// The event that fires when the specific attack instance successfully attacks.
    /// </summary>
    public event Action<AttackData> OnAttackSuccessful;

    /// <summary>
    /// Invokes the attack successful event.
    /// </summary>
    /// <param name="attackData">The attack data for the event</param>
    public void InvokeAttackSuccessful(AttackData attackData)
    {
        OnAttackSuccessful?.Invoke(attackData);
    }
}
