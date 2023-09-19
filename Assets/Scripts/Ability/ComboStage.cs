using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// POCO holding data for a stage in a combo.
/// </summary>
[Serializable]
public class ComboStage
{
    [SerializeField]
    private OnUseAbility ability;
    public OnUseAbility Ability => ability;

    /// <summary>
    /// The time at the end of the attack that can be canceled into the next combo, in seconds.
    /// </summary>
    [SerializeField]
    private float comboCancelableDuration;
    public float ComboCancelableDuration => comboCancelableDuration;

    /// <summary>
    /// Window to continue the combo in seconds.
    /// </summary>
    [SerializeField]
    private float comboContinueWindow;
    public float ComboContinueWindow => comboContinueWindow;
}
