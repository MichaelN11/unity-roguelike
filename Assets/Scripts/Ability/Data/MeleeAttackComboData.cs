using System;
using UnityEngine;

/// <summary>
/// POCO containing data for a specific stage in a melee attack combo.
/// </summary>
[Serializable]
public class MeleeAttackComboData
{
    [SerializeField]
    private PrefabAbilityData prefabAbilityData;
    public PrefabAbilityData PrefabAbilityData => prefabAbilityData;

    [SerializeField]
    private AttackAbilityData attackAbilityData;
    public AttackAbilityData AttackAbilityData => attackAbilityData;

    [SerializeField]
    private MovementAbilityData movementAbilityData;
    public MovementAbilityData MovementAbilityData => movementAbilityData;

    [SerializeField]
    private float castTime = 0f;
    public float CastTime => castTime;

    [SerializeField]
    private float comboableAttackDuration = 0f;
    public float ComboableAttackDuration => comboableAttackDuration;

    [SerializeField]
    private float comboContinueWindow = 1f;
    public float ComboContinueWindow => comboContinueWindow;
}
