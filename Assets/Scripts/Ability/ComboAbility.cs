using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An ability that has a combo of different abilities that can be used in succession.
/// </summary>
[CreateAssetMenu(menuName = "Game Data/Ability/Combo")]
public class ComboAbility : ActiveAbility
{
    [SerializeField]
    private List<ComboStage> comboStages;
    public List<ComboStage> ComboStages => comboStages;

    public void Use(int stage, EffectData abilityUse)
    {
        if (stage >= 0 && stage < comboStages.Count)
        {
            comboStages[stage].Ability.Use(abilityUse);
        }
    }
}
