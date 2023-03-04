using System;

/// <summary>
/// Class wrapping the ability events.
/// </summary>
public class AbilityEvents
{
    public event Action<AbilityUseEventInfo> OnAbilityUse;

    public void RaiseAbilityUseEvent(AbilityUseEventInfo abilityUseEventInfo)
    {
        OnAbilityUse?.Invoke(abilityUseEventInfo);
    }
}
