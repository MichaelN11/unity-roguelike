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
    private Attack attack;
    private Movement movement;
    private Damageable damageable;

    private Vector2 attemptedMoveDirection = Vector2.zero;
    private Vector2 attemptedLookDirection = Vector2.zero;

    private void Awake()
    {
        movement = GetComponent<Movement>();
        attack = GetComponentInChildren<Attack>();
        animatorUpdater = GetComponent<AnimatorUpdater>();
        damageable = GetComponent<Damageable>();
        InitializeComponents();
    }

    private void Update()
    {
        UpdateStunTimer();
        if (animatorUpdater != null)
        {
            animatorUpdater.UpdateAnimator(EntityData);
        }
        if (EntityData.ActionState == ActionState.Dead)
        {
            Debug.Log("he's dead Jim");
        }
    }

    /// <summary>
    /// Updates the entity based on the passed InputData.
    /// </summary>
    /// <param name="inputData">The input data containing the input type</param>
    public void UpdateFromInput(InputData inputData)
    {
        switch (inputData.Type)
        {
            case InputType.Look:
                SetLookDirection(inputData.Direction);
                break;
            case InputType.Move:
                SetMovementDirection(inputData.Direction);
                break;
            case InputType.Attack:
                Attack();
                break;
            case InputType.Idle:
                Idle();
                break;
            default:
                // Unrecognized input
                break;
        }
    }

    /// <summary>
    /// Handles being hit by an incoming attack.
    /// </summary>
    /// <param name="attackData">The attack data</param>
    public void HandleIncomingAttack(AttackData attackData)
    {
        if (damageable != null)
        {
            damageable.TakeDamage(attackData.AttackType.Damage);
            AttackResult attackResult = new();
            attackResult.HitStunDuration = EntityType.HitStunDuration * attackData.AttackType.HitStunMultiplier;
            attackResult.KnockbackSpeed = EntityType.KnockbackSpeed * attackData.AttackType.KnockbackMultiplier;
            attackResult.KnockbackDirection = attackData.Direction;
            attackResult.KnockbackAcceleration = EntityType.KnockbackAcceleration;

            if (damageable.IsDead())
            {
                Die();
            } else
            {
                HandleHitstun(attackResult);
            }
        }
    }

    /// <summary>
    /// Gets the attack range for the entity, with the interaction distance added.
    /// </summary>
    /// <returns>The attack range as a float</returns>
    public float GetAttackRange()
    {
        float range = 0;
        if (attack != null)
        {
            range = entityType.InteractionDistance + attack.AttackType.Range;
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
    private void SetMovementDirection(Vector2 moveDirection)
    {
        if (moveDirection != null)
        {
            attemptedMoveDirection = moveDirection;
            if (movement != null && CanAct())
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
            }
        }
    }

    /// <summary>
    /// Sets the look direction to the passed vector2.
    /// </summary>
    /// <param name="lookDirection">The new direction to look</param>
    private void SetLookDirection(Vector2 lookDirection)
    {
        if (lookDirection != null)
        {
            attemptedLookDirection = lookDirection;
            if (CanAct())
            {
                EntityData.LookDirection = lookDirection;
            }
        }
    }

    /// <summary>
    /// Tells the entity to attack, if it's able to act.
    /// </summary>
    private void Attack()
    {
        if (attack != null && CanAct())
        {
            EntityData.StunTimer = attack.AttackType.AttackDuration;
            EntityData.ActionState = ActionState.Attack;
            if (movement != null)
            {
                movement.SetMovement(Vector2.zero, 0);
            }
            attack.Use(EntityData.LookDirection, entityType.InteractionDistance, entityType);
        }
    }

    /// <summary>
    /// Determines if the entity is stunned.
    /// </summary>
    /// <returns>true if the entity is stunned</returns>
    private bool IsStunned()
    {
        return EntityData.ActionState == ActionState.Attack
            || EntityData.ActionState == ActionState.Hitstun;
    }

    /// <summary>
    /// Determines if the entity is able to act.
    /// </summary>
    /// <returns>true if the entity can act</returns>
    private bool CanAct()
    {
        return !IsStunned()
            && EntityData.ActionState != ActionState.Dead;
    }

    /// <summary>
    /// If the entity is stunned/attacking, subtract the time in seconds passed from the last
    /// update. If the timer is below 0, tell the entity to change state. Also sets
    /// the move direction to any move direction attempted while the entity was stunned.
    /// </summary>
    private void UpdateStunTimer()
    {
        if (IsStunned())
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
    private void Idle()
    {
        if (CanAct())
        {
            EntityData.ActionState = ActionState.Idle;
            movement.SetMovement(Vector2.zero, entityType.WalkSpeed);
        }
    }

    /// <summary>
    /// Interrupts what the entity is currently doing.
    /// </summary>
    private void Interrupt()
    {
        if (attack != null)
        {
            attack.Interrupt();
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
    }
}
