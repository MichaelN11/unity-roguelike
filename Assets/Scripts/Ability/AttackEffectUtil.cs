using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Utility class for attack effects.
/// </summary>
public class AttackEffectUtil
{
    /// <summary>
    /// Instantiates a damage object for an effect.
    /// </summary>
    /// <param name="effectData">The context of how the effect is being used</param>
    /// <param name="attackEffectData">The effect's attack data</param>
    /// <param name="prefabEffectData">The effect's prefab data</param>
    /// <param name="userEntityData">The effect user's entity data</param>
    /// <returns>The instantiated damage object</returns>
    public static GameObject InstantiateDamageObject(EffectData effectData,
        AttackEffectData attackEffectData,
        PrefabEffectData prefabEffectData,
        AttackData attackData)
    {
        Vector2 distance = effectData.Direction.normalized * attackEffectData.AttackDistance;
        Vector3 position = effectData.Position + distance;
        Quaternion rotation = (prefabEffectData.RotatePrefab) ? UnityUtil.RotateTowardsVector(effectData.Direction) : Quaternion.identity;
        GameObject instance = Object.Instantiate(prefabEffectData.Prefab, position, rotation);

        DestroyTimer destroyTimer = instance.GetComponent<DestroyTimer>();
        destroyTimer.Duration = prefabEffectData.PrefabDuration;

        DamageObject damageObject = instance.GetComponent<DamageObject>();
        damageObject.AttackData = attackData;

        return instance;
    }

    /// <summary>
    /// Builds the AttackData object used to pass data to the methods handling the attack.
    /// </summary>
    /// <param name="effectData">The object containing data about how the effect was used</param>
    /// <param name="attackEffectData">The effect's attack data</param>
    /// <returns>The AttackData object</returns>
    public static AttackData BuildAttackData(EffectData effectData,
        AttackEffectData attackEffectData)
    {
        AttackData attackData = new();
        attackData.EffectData = attackEffectData;
        attackData.User = UnityUtil.GetParentIfExists(effectData.Entity);
        attackData.Direction = effectData.Direction;
        attackData.SetDirectionOnHit = false;
        attackData.UserEntityData = effectData.EntityData;
        attackData.UserEntityState = effectData.EntityState;
        return attackData;
    }
}
