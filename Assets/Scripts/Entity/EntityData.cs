using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component for storing entity data.
/// </summary>
public class EntityData : MonoBehaviour
{
    private Entity entity;
    public Entity Entity => entity;

    private Faction faction;
    public Faction Faction => faction;

    private List<Faction> enemyFactions;
    public List<Faction> EnemyFactions => enemyFactions;

    /// <summary>
    /// Creates a new EntityData component and adds it to the passed object.
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="entity"></param>
    /// <param name="faction"></param>
    /// <param name="enemyFactions"></param>
    /// <returns></returns>
    public static EntityData AddToObject(GameObject gameObject, Entity entity,
        Faction faction, List<Faction> enemyFactions)
    {
        EntityData entityData = gameObject.AddComponent<EntityData>();
        entityData.entity = entity;
        entityData.faction = faction;
        entityData.enemyFactions = enemyFactions;
        return entityData;
    }
}
