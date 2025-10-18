using System;
using UnityEngine;

/// <summary>
/// Component for taking damage and managing health.
/// </summary>
public class Damageable : MonoBehaviour
{
    private const float PlayerInvincibilityTimeOnHit = 0.5f;

    public event Action OnDamageTaken;

    [field: SerializeField]
    public float MaxHealth { get; private set; }
    public float CurrentHealth { get; private set; }

    private EntityData entityData;
    public EntityData EntityData => entityData;

    [SerializeField]
    private float destroyTimer = 0;

    [SerializeField]
    private Sound destroySound;

    private EntityState entityState;
    private Movement movement;
    private LevelObject levelObject;
    private Animator animator;

    private float invincibilityTimer = 0;
    private bool isDead = false;

    private EntityDescription lastHitByEntity;
    private AttackDescription lastHitByAttack;

    private void Awake()
    {
        entityData = GetComponent<EntityData>();
        entityState = GetComponent<EntityState>();
        movement = GetComponent<Movement>();
        levelObject = GetComponent<LevelObject>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (MaxHealth <= 0 && entityData != null)
        {
            MaxHealth = entityData.Entity.MaxHealth;
        }
        if (CurrentHealth <= 0f)
        {
            CurrentHealth = MaxHealth;
        }
    }

    private void Update()
    {
        if (entityState == null || !entityState.IsStopped())
        {
            if (invincibilityTimer > 0)
            {
                invincibilityTimer -= Time.deltaTime;
            }

            if (IsDying() && !isDead)
            {
                Die();
            }
        }
    }

    /// <summary>
    /// Creates a new Damageable component and adds it to the passed object.
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="maxHealth"></param>
    /// <param name="currentHealth"></param>
    /// <returns></returns>
    public static Damageable AddToObject(GameObject gameObject, float maxHealth, float currentHealth = 0)
    {
        Damageable damageable = gameObject.AddComponent<Damageable>();
        damageable.MaxHealth = maxHealth;
        if (currentHealth > 0)
        {
            damageable.CurrentHealth = currentHealth;
        } else
        {
            damageable.CurrentHealth = maxHealth;
        }
        return damageable;
    }

    /// <summary>
    /// Handles being hit by an incoming attack.
    /// </summary>
    /// <param name="attackData">The attack data</param>
    public void HandleIncomingAttack(AttackData attackData)
    {
        if (invincibilityTimer > 0)
        {
            return;
        }

        TakeDamage(attackData.Damage);
        if (entityState != null && entityData != null)
        {
            entityState.Stop(attackData.HitStop);
            entityState.Flash(entityData.Entity.FlashOnHitTime);
            AudioManager.Instance.Play(entityData.Entity.SoundOnHit);
            AttackResult attackResult = new();
            if (attackData.StunPower > entityData.Entity.Poise)
            {
                attackResult.HitStunDuration = entityData.Entity.HitStunDuration * attackData.HitStunMultiplier;
            } else
            {
                attackResult.HitStunDuration = 0;
            }
            attackResult.KnockbackSpeed = entityData.Entity.KnockbackSpeed * attackData.KnockbackMultiplier;
            attackResult.KnockbackDirection = attackData.Direction;
            attackResult.KnockbackAcceleration = entityData.Entity.KnockbackAcceleration;
            HandleHitstun(attackResult);
        } else if (levelObject != null)
        {
            attackData.TargetIsObject = true;  
        }

        if (CompareTag("Player"))
        {
            SetInvincibility(PlayerInvincibilityTimeOnHit);
        }

        if (attackData.UserEntityData != null)
        {
            lastHitByEntity = attackData.UserEntityData.Entity.Description;
        }
        lastHitByAttack = attackData.Description;
    }

    public void SetInvincibility(float duration)
    {
        invincibilityTimer = duration;
    }

    public void Heal(float amount)
    {
        CurrentHealth += amount;
        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }
    }

    public void IncreaseMaxHealth(float amount)
    {
        MaxHealth += amount;
        CurrentHealth += amount;
    }

    /// <summary>
    /// Handles the hitstun and knockback after being hit by an attack.
    /// </summary>
    /// <param name="attackResult">The results of the attack</param>
    private void HandleHitstun(AttackResult attackResult)
    {
        if (attackResult.HitStunDuration > 0)
        {
            entityState.HitstunState(attackResult.HitStunDuration);
            if (movement != null)
            {
                movement.SetMovement(attackResult.KnockbackDirection,
                    attackResult.KnockbackSpeed, attackResult.KnockbackAcceleration);
            }
        }
    }

    /// <summary>
    /// Kills the entity.
    /// </summary>
    private void Die()
    {
        isDead = true;

        DisableColliders();
        DropObjects();
        RemoveShadow();

        if (movement != null)
        {
            movement.StopMoving();
        }

        if (entityState != null && entityData != null)
        {
            if (entityData.Entity.SoundOnDeath != null)
            {
                AudioManager.Instance.Play(entityData.Entity.SoundOnDeath);
            }
            DeathContext deathContext = new();
            deathContext.KillingEntity = lastHitByEntity;
            deathContext.KillingAttack = lastHitByAttack;
            entityState.DeadState(deathContext);
            Destroy(gameObject, entityData.Entity.DeathTimer);
        } else if (animator != null)
        {
            if (destroySound != null)
            {
                AudioManager.Instance.Play(destroySound);
            }
            animator.SetTrigger("destroy");
            Destroy(gameObject, destroyTimer);
        } else
        {
            Destroy(gameObject);
        }
    }

    private void DropObjects()
    {
        if (CompareTag("Player"))
        {
            return;
        }

        InventoryItem inventoryItem = null;
        if (entityData != null && entityData.Entity.ItemDrops != null)
        {
            ItemDrop itemDrop = ItemDropUtil.GetRandomItemDrop(entityData.Entity.ItemDrops);
            if (itemDrop != null)
            {
                inventoryItem = itemDrop.InventoryItem;
            }
        }
        else if (levelObject != null && levelObject.ContainedItem != null)
        {
            inventoryItem = levelObject.ContainedItem;
        }

        if (inventoryItem != null)
        {
            DropItem(inventoryItem);
        }
    }

    private void DropItem(InventoryItem inventoryItem)
    {
        if (inventoryItem.Item)
        {
            GameObject dropPrefab = (inventoryItem.Item.DropPrefab) ? inventoryItem.Item.DropPrefab : ResourceManager.Instance.ItemPickupObject;
            GameObject droppedItem = Instantiate(dropPrefab, this.transform.position, Quaternion.identity);
            droppedItem.GetComponent<ItemPickup>().Init(inventoryItem.Item, inventoryItem.Amount);
        }
    }

    private void DisableColliders()
    {
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D collider in colliders)
        {
            collider.enabled = false;
        }
    }

    private void RemoveShadow()
    {
        Transform shadow = transform.Find("Shadow");
        if (shadow != null)
        {
            Destroy(shadow.gameObject);
        }
    }

    /// <summary>
    /// Take damage to health.
    /// </summary>
    /// <param name="damage">The damage being dealt</param>
    private void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        OnDamageTaken?.Invoke();
    }

    /// <summary>
    /// Determines if the object is dying.
    /// </summary>
    /// <returns>true if the object is dying</returns>
    private bool IsDying()
    {
        return CurrentHealth <= 0;
    }
}
