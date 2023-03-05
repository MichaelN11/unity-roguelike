using System;

/// <summary>
/// Container for holding events for a specific entity's abilities.
/// </summary>
public class AbilityEvents
{
    /// <summary>
    /// The event that fires when an ability is used by the entity.
    /// </summary>
    public event Action<AbilityUseEventInfo> OnAbilityUse;

    public void InvokeAbilityUseEvent(AbilityUseEventInfo abilityUseEventInfo)
    {
        OnAbilityUse?.Invoke(abilityUseEventInfo);
    }
}
