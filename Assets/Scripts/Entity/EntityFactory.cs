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
        return CreatePlayer(GameManager.Instance.AddressableService.RetrieveEntity(entityName), position);
    }

    /// <summary>
    /// Creates a new instance of an entity as a player, at the passed location, using the passed entity name.
    /// </summary>
    /// <param name="entityName"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public static GameObject CreateEnemy(string entityName, Vector2 position)
    {
        return CreateEnemy(GameManager.Instance.AddressableService.RetrieveEntity(entityName), position);
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
        CreateNewCommonComponents(entity, player);
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
        CreateNewCommonComponents(entity, enemy);
        AIController.AddToObject(enemy, entity.EntityAI);

        enemy.SetActive(true);
        return enemy;
    }

    /// <summary>
    /// Loads the player entity from the passed entity save data.
    /// </summary>
    /// <param name="saveData"></param>
    /// <returns></returns>
    public static GameObject LoadPlayer(EntitySave saveData)
    {
        Entity entity = GameManager.Instance.AddressableService.RetrieveEntity(saveData.Name);
        GameObject player = GameObject.Instantiate(entity.BaseObject, saveData.Position, Quaternion.identity);
        player.SetActive(false);

        EntityData.AddToObject(player, entity, Faction.Player, PlayerEnemies);
        LoadCommonComponents(entity, player, saveData);
        player.AddComponent<PlayerController>();
        player.tag = "Player";

        player.SetActive(true);
        return player;
    }

    /// <summary>
    /// Loads the enemy entity from the passed entity save data.
    /// </summary>
    /// <param name="saveData"></param>
    /// <returns></returns>
    public static GameObject LoadEnemy(EntitySave saveData)
    {
        Entity entity = GameManager.Instance.AddressableService.RetrieveEntity(saveData.Name);
        GameObject enemy = GameObject.Instantiate(entity.BaseObject, saveData.Position, Quaternion.identity);
        enemy.SetActive(false);

        EntityData.AddToObject(enemy, entity, Faction.Enemy, EnemyEnemies);
        LoadCommonComponents(entity, enemy, saveData);
        AIController.AddToObject(enemy, entity.EntityAI);

        enemy.SetActive(true);
        return enemy;
    }

    /// <summary>
    /// Creates new components for a newly created entity.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="entityObject"></param>
    private static void CreateNewCommonComponents(Entity entity, GameObject entityObject)
    {
        Damageable.AddToObject(entityObject, entity.MaxHealth);
        Inventory.AddToObject(entityObject, entity.InitialInventory);
        BuildCommonComponents(entity, entityObject);
    }

    /// <summary>
    /// Loads the common components for an entity from the passed save data. 
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="entityObject"></param>
    /// <param name="saveData"></param>
    private static void LoadCommonComponents(Entity entity, GameObject entityObject, EntitySave saveData)
    {
        float maxHealth = (saveData.MaxHealth > 0) ? saveData.MaxHealth : entity.MaxHealth;
        if (saveData.CurrentHealth > 0)
        {
            Damageable.AddToObject(entityObject, maxHealth, saveData.CurrentHealth);
        }
        else
        {
            Damageable.AddToObject(entityObject, maxHealth);
        }

        List<InventoryItem> inventoryItems = new();
        foreach (InventoryItemSave itemSave in saveData.InventoryItems)
        {
            InventoryItem inventoryItem = ItemFactory.LoadItem(itemSave);
            inventoryItems.Add(inventoryItem);
        }
        Inventory.AddToObject(entityObject, inventoryItems);

        BuildCommonComponents(entity, entityObject);
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
        Transform hitboxTransform = entityObject.transform.Find("Hitbox");
        if (hitboxTransform != null)
        {
            hitboxTransform.gameObject.AddComponent<Hitbox>();
        }
    }
}
