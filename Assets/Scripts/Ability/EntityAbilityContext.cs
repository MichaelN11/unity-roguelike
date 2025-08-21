using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class containing context about an entity's current ability usage.
/// </summary>
public class EntityAbilityContext
{
    public float PreviousCancelableDuration { get; set; } = 0;

    public ComboAbility CurrentComboAbility { get; set; }
    public int NextComboNumber { get; set; } = 0;
    public float ComboTimer { get; set; } = 0;
    public float ComboableTime { get; set; } = 0;

    public ActiveAbility CurrentActiveAbility { get; set; }
    public AbilityUseData CurrentAbilityData { get; set; }
    public float CurrentAbilityDuration { get; set; }
    public bool CurrentAbilityStarted { get; set; } = false; // Set when the startup/cast time is finished
    public string CurrentAbilityOrigin { get; set; } = null;

    public bool IsAbilityCharging { get; set; } = false;
    public float ChargeTimer { get; set; } = 0;

    public IEnumerator DelayedAbilityCoroutine { get; set; }
}
