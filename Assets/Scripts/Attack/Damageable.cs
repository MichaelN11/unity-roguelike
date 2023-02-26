using UnityEngine;

/// <summary>
/// Component for managing health.
/// </summary>
public class Damageable : MonoBehaviour
{
    public float MaxHealth { get; set; }
    public float CurrentHealth { get; private set; }

    private EntityData entityData;
    private EntityState entityState;
    private Movement movement;

    private void Awake()
    {
        entityData = GetComponent<EntityData>();
        entityState = GetComponent<EntityState>();
        movement = GetComponent<Movement>();
    }

    private void Start()
    {
        if (entityData != null)
        {
            MaxHealth = entityData.EntityType.MaxHealth;
        }
        CurrentHealth = MaxHealth;
    }

    private void Update()
    {
        if ((entityState == null
                || !entityState.IsStopped())
            && IsDead())
        {
            Die();
        }
    }

    /// <summary>
    /// Gets the EntityType from the EntityData.
    /// </summary>
    /// <returns>The EntityType</returns>
    public EntityType GetEntityType()
    {
        EntityType entityType = null;
        if (entityData != null)
        {
            entityType = entityData.EntityType;
        }
        return entityType;
    }

    /// <summary>
    /// Handles being hit by an incoming attack.
    /// </summary>
    /// <param name="attackData">The attack data</param>
    public void HandleIncomingAttack(AttackData attackData)
    {
        TakeDamage(attackData.AbilityData.Damage);
        if (entityState != null && entityData != null)
        {
            entityState.Stop(attackData.AbilityData.HitStop);
            entityState.Flash(entityData.EntityType.FlashOnHitTime);
            AudioManager.Instance.Play(entityData.EntityType.SoundOnHit);
            AttackResult attackResult = new();
            attackResult.HitStunDuration = entityData.EntityType.HitStunDuration * attackData.AbilityData.HitStunMultiplier;
            attackResult.KnockbackSpeed = entityData.EntityType.KnockbackSpeed * attackData.AbilityData.KnockbackMultiplier;
            attackResult.KnockbackDirection = attackData.Direction;
            attackResult.KnockbackAcceleration = entityData.EntityType.KnockbackAcceleration;
            HandleHitstun(attackResult);
        }
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
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D collider in colliders)
        {
            collider.enabled = false;
        }

        if (movement != null)
        {
            movement.StopMoving();
        }

        if (entityState != null && entityData != null)
        {
            entityState.DeadState();
            Destroy(gameObject, entityData.EntityType.DeathTimer);
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
    /// Determines if the object is dead.
    /// </summary>
    /// <returns>true if the object is dead</returns>
    private bool IsDead()
    {
        return CurrentHealth <= 0;
    }
}
