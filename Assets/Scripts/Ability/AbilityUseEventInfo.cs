using System;

/// <summary>
/// POCO containing data to pass in the ability use event.
/// </summary>
public class AbilityUseEventInfo
{
    public AbilityUseData AbilityUse { get; set; }
    public AbilityAnimation AbilityAnimation { get; set; }
    public float CastTime { get; set; }
    public float ActiveTime { get; set; }
    public float RecoveryTime { get; set; }
    public float AimDuration { get; set; }
    public float Range { get; set; }
    public bool ChangeDirection { get; set; }
    public bool StatelessCast { get; set; }

    public float GetFullDuration()
    {
        return CastTime + ActiveTime + RecoveryTime;
    }
}
