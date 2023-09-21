using UnityEngine;

/// <summary>
/// Component for taking damage and managing health.
/// </summary>
public class Damageable : MonoBehaviour
{
    private const float PlayerInvincibilityTimeOnHit = 1;

    public float MaxHealth { get; private set; }
    public float CurrentHealth { get; private set; }

    private EntityData entityData;
    public EntityData EntityData => entityData;

    private EntityState entityState;
    private Movement movement;

    private float invincibilityTimer = 0;

    private bool isDead = false;

    private void Awake()
    {
        entityData = GetComponent<EntityData>();
        entityState = GetComponent<EntityState>();
        movement = GetComponent<Movement>();
    }

    private void Start()
    {
        if (MaxHealth <= 0 && entityData != null)
        {
            MaxHealth = entityData.Entity.MaxHealth;
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

        TakeDamage(attackData.EffectData.Damage);
        if (entityState != null && entityData != null)
        {
            entityState.Stop(attackData.EffectData.HitStop);
            entityState.Flash(entityData.Entity.FlashOnHitTime);
            AudioManager.Instance.Play(entityData.Entity.SoundOnHit);
            AttackResult attackResult = new();
            attackResult.HitStunDuration = entityData.Entity.HitStunDuration * attackData.EffectData.HitStunMultiplier;
            attackResult.KnockbackSpeed = entityData.Entity.KnockbackSpeed * attackData.EffectData.KnockbackMultiplier;
            attackResult.KnockbackDirection = attackData.Direction;
            attackResult.KnockbackAcceleration = entityData.Entity.KnockbackAcceleration;
            HandleHitstun(attackResult);
        }

        if (CompareTag("Player"))
        {
            SetInvincibility(PlayerInvincibilityTimeOnHit);
        }
    }

    public void SetInvincibility(float duration)
    {
        invincibilityTimer = duration;
    }

    public void Heal(float amount)
    {
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

        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D collider in colliders)
        {
            collider.enabled = false;
        }

        if (entityData.Entity.DropChance > 0)
        {
            if (Random.value <= entityData.Entity.DropChance)
            {
                Debug.Log("Dropping heart on death!");
                Instantiate(entityData.Entity.Droppable, this.transform.position, Quaternion.identity);
            }
        }

        Transform shadow = transform.Find("Shadow");
        if (shadow != null)
        {
            Destroy(shadow.gameObject);
        }

        if (movement != null)
        {
            movement.StopMoving();
        }

        if (entityState != null && entityData != null)
        {
            entityState.DeadState();
            Destroy(gameObject, entityData.Entity.DeathTimer);
        } else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Take damage to health.
    /// </summary>
    /// <param name="damage">The damage being dealt</param>
    private void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
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
