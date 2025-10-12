using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An ability that can change based on certain conditions.
/// </summary>
[CreateAssetMenu(menuName = "Game Data/Ability/Conditional")]
public class ConditionalAbility : ActiveAbility
{
    /// <summary>
    /// A list of abililties that can activate. The first one that meets its conditions will be used.
    /// </summary>
    [SerializeField]
    List<ActiveAbility> orderedAbilities = new();

    public override AbilityUseEventInfo Use(Vector2 direction, AbilityUseData abilityUse, EntityAbilityContext entityAbilityContext)
    {
        ActiveAbility ability = GetActiveAbility(abilityUse, entityAbilityContext);
        if (ability != null)
        {
            return ability.Use(direction, abilityUse, entityAbilityContext);
        }
        return null;
    }

    public override bool CanActivate(AbilityUseData abilityUse, EntityAbilityContext entityAbilityContext)
    {
        foreach (ActiveAbility ability in orderedAbilities)
        {
            if (ability.CanActivate(abilityUse, entityAbilityContext))
            {
                return true;
            }
        }
        return false;
    }

    public override UsableAbilityInfo GetUsableAbilityInfo(AbilityUseData abilityUse, EntityAbilityContext entityAbilityContext)
    {
        ActiveAbility ability = GetActiveAbility(abilityUse, entityAbilityContext);
        if (ability != null)
        {
            return ability.GetUsableAbilityInfo(abilityUse, entityAbilityContext);
        }
        return new UsableAbilityInfo();
    }

    private ActiveAbility GetActiveAbility(AbilityUseData abilityUse, EntityAbilityContext entityAbilityContext)
    {
        foreach (ActiveAbility ability in orderedAbilities)
        {
            if (ability.CanActivate(abilityUse, entityAbilityContext))
            {
                return ability;
            }
        }
        return null;
    }
}
