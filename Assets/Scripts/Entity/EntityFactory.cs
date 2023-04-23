using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Factory class for creating new entity instances.
/// </summary>
public class EntityFactory
{
    private static readonly List<Faction> PlayerEnemies = new() { Faction.Enemy };
    private static readonly List<Faction> EnemyEnemies = new() { Faction.Player };

    /// <summary>
    /// Creates a new instance of an entity as a player, at the passed location, using the passed entity name.
    /// </summary>
    /// <param name="entityName"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public static GameObject CreatePlayer(string entityName, Vector2 position)
    {
        return CreatePlayer(AddressableService.LoadEntity(entityName), position);
    }

    /// <summary>
    /// Creates a new instance of an entity as a player, at the passed location, using the passed entity name.
    /// </summary>
    /// <param name="entityName"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public static GameObject CreateEnemy(string entityName, Vector2 position)
    {
        return CreateEnemy(AddressableService.LoadEntity(entityName), position);
    }

    /// <summary>
    /// Creates a new instance of the passed entity as a player, at the passed location.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public static GameObject CreatePlayer(Entity entity, Vector2 position)
    {
        GameObject player = GameObject.Instantiate(entity.BaseObject, position, Quaternion.identity);
        player.SetActive(false);

        EntityData.AddToObject(player, entity, Faction.Player, PlayerEnemies);
        BuildCommonComponents(entity, player);
        player.AddComponent<PlayerController>();
        player.tag = "Player";

        player.SetActive(true);
        return player;
    }

    /// <summary>
    /// Creates a new instance of the passed entity as an enemy, at the passed location.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public static GameObject CreateEnemy(Entity entity, Vector2 position)
    {
        GameObject enemy = GameObject.Instantiate(entity.BaseObject, position, Quaternion.identity);
        enemy.SetActive(false);

        EntityData.AddToObject(enemy, entity, Faction.Enemy, EnemyEnemies);
        BuildCommonComponents(entity, enemy);
        AIController.AddToObject(enemy, entity.EntityAI);

        enemy.SetActive(true);
        return enemy;
    }

    /// <summary>
    /// Builds the common entity components for the passed entity game object.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="entityObject"></param>
    private static void BuildCommonComponents(Entity entity, GameObject entityObject)
    {
        entityObject.AddComponent<EntityState>();
        entityObject.AddComponent<EntityController>();
        entityObject.AddComponent<Damageable>();
        entityObject.AddComponent<Movement>();
        entityObject.AddComponent<AnimatorUpdater>();

        Transform abilityTransform = entityObject.transform.Find("AbilitySource");
        if (abilityTransform != null)
        {
            AbilityManager.AddToObject(abilityTransform.gameObject, entity.Abilities);
        }
        Transform weaponTransform = entityObject.transform.Find("Weapon");
        if (weaponTransform != null && entity.Weapon != null)
        {
            WeaponController.AddToObject(weaponTransform.gameObject, entity.Weapon);
        }
    }
}
