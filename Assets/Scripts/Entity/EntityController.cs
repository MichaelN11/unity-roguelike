using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component representing the main controller for an entity.
/// </summary>
public class EntityController : MonoBehaviour
{
    public EntityData EntityData { get; private set; } = new EntityData();

    [SerializeField]
    private EntityType entityType;
    public EntityType EntityType => entityType;

    private AnimatorUpdater animatorUpdater;
    private Movement movement;
    private Damageable damageable;
    private AbilityManager abilityManager;

    private Vector2 attemptedMoveDirection = Vector2.zero;
    private Vector2 attemptedLookDirection = Vector2.zero;

    private void Awake()
    {
        movement = GetComponent<Movement>();
        abilityManager = GetComponentInChildren<AbilityManager>();
        animatorUpdater = GetComponent<AnimatorUpdater>();
        damageable = GetComponent<Damageable>();

        InitializeComponents();
    }

    private void Update()
    {
        EntityData.MovementStopped = EntityData.IsStopped();

        if (EntityData.IsStopped())
        {
            UpdateStopTimer();
        }
        else
        {
            UpdateStunTimer();
            UpdateFlashTimer();
            if (damageable != null && damageable.IsDead())
            {
                Die();
            }
        }

        if (animatorUpdater != null)
        {
            animatorUpdater.UpdateAnimator(EntityData);
        }
    }

    /// <summary>
    /// Updates the entity based on the passed InputData.
    /// </summary>
    /// <param name="inputData">The input data containing the input type</param>
    /// <returns>true if the update was successful</returns>
    public bool UpdateFromInput(InputData inputData)
    {
        bool updateSuccessful = false;
        switch (inputData.Type)
        {
            case InputType.Look:
                updateSuccessful = SetLookDirection(inputData.Direction);
                break;
            case InputType.Move:
                updateSuccessful = SetMovementDirection(inputData.Direction);
                break;
            case InputType.Attack:
                updateSuccessful = Attack(inputData.Direction);
                break;
            case InputType.Idle:
                updateSuccessful = Idle();
                break;
            default:
                // Unrecognized input
                break;
        }
        return updateSuccessful;
    }

    /// <summary>
    /// Handles being hit by an incoming attack.
    /// </summary>
    /// <param name="attackData">The attack data</param>
    public void HandleIncomingAttack(AttackData attackData)
    {
        if (damageable != null)
        {
            EntityData.StopTimer = attackData.AbilityData.HitStop;
            EntityData.FlashTimer = entityType.FlashOnHitTime;
            AudioManager.Instance.Play(entityType.SoundOnHit);
            damageable.TakeDamage(attackData.AbilityData.Damage);
            AttackResult attackResult = new();
            attackResult.HitStunDuration = EntityType.HitStunDuration * attackData.AbilityData.HitStunMultiplier;
            attackResult.KnockbackSpeed = EntityType.KnockbackSpeed * attackData.AbilityData.KnockbackMultiplier;
            attackResult.KnockbackDirection = attackData.Direction;
            attackResult.KnockbackAcceleration = EntityType.KnockbackAcceleration;
            HandleHitstun(attackResult);
        }
    }

    /// <summary>
    /// Gets the attack range for the entity, with the interaction distance and radius added.
    /// </summary>
    /// <returns>The attack range as a float</returns>
    public float GetAttackRange()
    {
        float range = 0;
        if (abilityManager != null)
        {
            range = entityType.InteractionDistance + abilityManager.GetRange();
        }
        return range;
    }

    /// <summary>
    /// Kills the entity.
    /// </summary>
    private void Die()
    {
        Interrupt();
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D collider in colliders)
        {
            collider.enabled = false;
        }
        if (movement != null)
        {
            movement.SetMovement(Vector2.zero, 0);
        }
        EntityData.ActionState = ActionState.Dead;
        Destroy(gameObject, EntityType.DeathTimer);
    }

    /// <summary>
    /// Sets the movement direction for the passed direction vector. Sets the direction
    /// on the movement component, and changes the entity's state to match the new direction.
    /// </summary>
    /// <param name="moveDirection">The new direction to move</param>
    /// <returns>true if the movement was set</returns>
    private bool SetMovementDirection(Vector2 moveDirection)
    {
        bool isMovementSet = false;
        if (moveDirection != null)
        {
            attemptedMoveDirection = moveDirection;
            if (movement != null && EntityData.CanAct())
            {
                movement.SetMovement(moveDirection, entityType.WalkSpeed);

                if (moveDirection != Vector2.zero)
                {
                    EntityData.ActionState = ActionState.Move;
                }
                else
                {
                    EntityData.ActionState = ActionState.Stand;
                }

                isMovementSet = true;
            }
        }
        return isMovementSet;
    }

    /// <summary>
    /// Sets the look direction to the passed vector2.
    /// </summary>
    /// <param name="lookDirection">The new direction to look</param>
    /// <returns>true if the look direction was set</returns>
    private bool SetLookDirection(Vector2 lookDirection)
    {
        bool isLookDirectionSet = false;
        if (lookDirection != null)
        {
            attemptedLookDirection = lookDirection;
            if (EntityData.CanAct())
            {
                EntityData.LookDirection = lookDirection;
                isLookDirectionSet = true;
            }
        }
        return isLookDirectionSet;
    }

    /// <summary>
    /// Tells the entity to attack, if it's able to attack. Sets the look direction to
    /// the attack direction, and sets the attack stun duration. Also
    /// updates the AnimatorUpdator that a new attack has occured.
    /// </summary>
    /// <param name="attackDirection">The direction of the attack</param>
    /// <returns>true if the attack was successful</returns>
    private bool Attack(Vector2 attackDirection)
    {
        bool attackSuccessful = false;
        if (abilityManager != null)
        {
            AbilityUse abilityUse = new AbilityUse();
            abilityUse.User = EntityData;
            abilityUse.EntityType = EntityType;
            abilityUse.Direction = attackDirection;
            abilityUse.Position = abilityManager.transform.position + (Vector3) (attackDirection.normalized * entityType.InteractionDistance);
            attackSuccessful = abilityManager.UseAbility(abilityUse);

            if (attackSuccessful)
            {
                animatorUpdater.HasAttacked = false;
            }
        }
        return attackSuccessful;
    }

    /// <summary>
    /// If the entity is stunned/attacking, subtract the time in seconds passed from the last
    /// update. If the timer is below 0, tell the entity to change state. Also sets
    /// the move direction to any move direction attempted while the entity was stunned.
    /// </summary>
    private void UpdateStunTimer()
    {
        if (EntityData.IsStunned())
        {
            EntityData.StunTimer -= Time.deltaTime;
            if (EntityData.StunTimer <= 0)
            {
                EntityData.ActionState = ActionState.Stand;
                SetMovementDirection(attemptedMoveDirection);
                SetLookDirection(attemptedLookDirection);
            }
        }
    }

    /// <summary>
    /// Updates the flash timer, which controls how long the entity is flashing white.
    /// </summary>
    private void UpdateFlashTimer()
    {
        if (EntityData.IsFlashing())
        {
            EntityData.FlashTimer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Updates the stop timer, which controls how long the entity is stopped (paused).
    /// </summary>
    private void UpdateStopTimer()
    {
        EntityData.StopTimer -= Time.deltaTime;
    }

    /// <summary>
    /// Handles the hitstun and knockback after being hit by an attack.
    /// </summary>
    /// <param name="attackResult">The results of the attack</param>
    private void HandleHitstun(AttackResult attackResult)
    {
        if (attackResult.HitStunDuration > 0)
        {
            Interrupt();
            EntityData.ActionState = ActionState.Hitstun;
            EntityData.StunTimer = attackResult.HitStunDuration;
            if (movement != null)
            {
                movement.SetMovement(attackResult.KnockbackDirection,
                    attackResult.KnockbackSpeed, attackResult.KnockbackAcceleration);
            }
        }
    }

    /// <summary>
    /// Makes the entity go idle.
    /// </summary>
    /// <returns>true if the idle action was successful</returns>
    private bool Idle()
    {
        bool isIdleSet = false;
        if (EntityData.CanAct())
        {
            EntityData.ActionState = ActionState.Idle;
            movement.SetMovement(Vector2.zero, entityType.WalkSpeed);
            isIdleSet = true;
        }
        return isIdleSet;
    }

    /// <summary>
    /// Interrupts what the entity is currently doing.
    /// </summary>
    private void Interrupt()
    {
        if (abilityManager != null)
        {
            abilityManager.Interrupt();
        }
    }

    /// <summary>
    /// Initializes the entity's components with the entity data.
    /// </summary>
    private void InitializeComponents()
    {
        AttackOnCollision attackOnCollision = GetComponentInChildren<AttackOnCollision>();
        if (attackOnCollision != null)
        {
            attackOnCollision.attackData.EntityType = entityType;
        }
        if (damageable != null)
        {
            damageable.MaxHealth = EntityType.MaxHealth;
        }
        if (movement != null)
        {
            movement.EntityData = EntityData;
        }
    }
}
