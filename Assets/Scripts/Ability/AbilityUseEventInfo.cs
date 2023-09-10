using System;

/// <summary>
/// POCO containing data to pass in the ability use event.
/// </summary>
public class AbilityUseEventInfo
{
    public EffectData AbilityUse { get; set; }
    public AbilityAnimation AbilityAnimation { get; set; }
}
