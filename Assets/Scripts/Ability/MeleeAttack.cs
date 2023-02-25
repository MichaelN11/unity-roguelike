using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ability scriptable object representing a melee attack.
/// </summary>
[CreateAssetMenu(menuName = "Game Data/Ability/Melee Attack")]
public class MeleeAttack : Ability
{
    [SerializeField]
    private List<MeleeAttackComboData> comboDataList;
    public List<MeleeAttackComboData> ComboDataList => comboDataList;

    public override AbilityBehavior BuildBehavior(AbilityManager abilityManager)
    {
        return new MeleeAttackBehavior(this, abilityManager);
    }

    /// <summary>
    /// Unity's Reset method. Used to initialize with default values in the inspector.
    /// </summary>
    private void Reset()
    {
        comboDataList = new List<MeleeAttackComboData>()
        {
            new MeleeAttackComboData()
        };
    }
}
