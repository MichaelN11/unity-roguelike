using System;
using UnityEngine;

/// <summary>
/// An AbilityEffect that causes the entity to pass through entities.
/// </summary>
[CreateAssetMenu(menuName = "Game Data/Ability Effect/Pass Through Entities")]
public class PassThroughEntitiesEffect : AbilityEffect
{
    public override void Trigger(AbilityUseData abilityUseData, EffectUseData effectUseData)
    {
        abilityUseData.Movement.PassThroughEntities(Duration);
    }

    public override void Unapply(AbilityUseData abilityUseData, EffectUseData effectUseData)
    {
        abilityUseData.Movement.StopPassingThroughEntities();
    }
}
