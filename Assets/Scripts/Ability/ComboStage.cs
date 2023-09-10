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

    [SerializeField]
    private float comboableDuration;
    public float ComboableDuration => comboableDuration;

    [SerializeField]
    private float comboContinueWindow;
    public float ComboContinueWindow => comboContinueWindow;
}
