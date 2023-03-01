using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Utility class for attack abilities.
/// </summary>
public class AttackAbilityUtil
{
    /// <summary>
    /// Instantiates a damage object for an ability.
    /// </summary>
    /// <param name="abilityUse">The context of how the ability is being used</param>
    /// <param name="attackAbilityData">The ability's attack data</param>
    /// <param name="prefabAbilityData">The ability's prefab data</param>
    /// <param name="userEntityData">The ability user's entity data</param>
    /// <returns>The instantiated damage object</returns>
    public static GameObject InstantiateDamageObject(AbilityUse abilityUse,
        AttackAbilityData attackAbilityData,
        PrefabAbilityData prefabAbilityData,
        AttackData attackData)
    {
        Vector2 distance = abilityUse.Direction.normalized * attackAbilityData.Range;
        Vector3 position = abilityUse.Position + distance;
        Quaternion rotation = (prefabAbilityData.RotatePrefab) ? UnityUtil.RotateTowardsVector(abilityUse.Direction) : Quaternion.identity;
        GameObject instance = Object.Instantiate(prefabAbilityData.Prefab, position, rotation);

        DestroyTimer destroyTimer = instance.GetComponent<DestroyTimer>();
        destroyTimer.Duration = prefabAbilityData.PrefabDuration;

        DamageObject damageObject = instance.GetComponent<DamageObject>();
        damageObject.AttackData = attackData;

        AudioManager.Instance.Play(attackAbilityData.SoundOnUse);

        return instance;
    }

    /// <summary>
    /// Builds the AttackData object used to pass data to the methods handling the attack.
    /// </summary>
    /// <param name="abilityUse">The object containing data about how the ability was used</param>
    /// <param name="attackAbilityData">The ability's attack data</param>
    /// <param name="userEntityData">The user of the ability</param>
    /// <returns>The AttackData object</returns>
    public static AttackData BuildAttackData(AbilityUse abilityUse,
        AttackAbilityData attackAbilityData,
        EntityData userEntityData)
    {
        AttackData attackData = new();
        attackData.AbilityData = attackAbilityData;
        attackData.User = UnityUtil.GetParentIfExists(abilityUse.Component.gameObject);
        attackData.Direction = abilityUse.Direction;
        attackData.SetDirectionOnHit = false;
        attackData.EntityData = userEntityData;
        return attackData;
    }
}
