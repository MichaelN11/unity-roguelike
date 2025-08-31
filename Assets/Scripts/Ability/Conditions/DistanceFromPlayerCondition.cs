using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Condition that checks if an entity is within a specified distance range from the player.
/// </summary>
[CreateAssetMenu(menuName = "Game Data/Condition/DistanceFromPlayer")]
public class DistanceFromPlayerCondition : AbilityCondition
{
    [SerializeField]
    private float minimumDistance = 0;
    [SerializeField]
    private float maximumDistance = float.PositiveInfinity;

    public override bool ConditionMet(AbilityUseData abilityUseData, EntityAbilityContext entityAbilityContext)
    {
        Vector2 playerPosition = PlayerController.Instance.transform.position;
        Vector2 userPosition = abilityUseData.AbilityManager.transform.position;

        float distance = Vector2.Distance(playerPosition, userPosition);
        return distance >= minimumDistance && distance <= maximumDistance;
    }
}
