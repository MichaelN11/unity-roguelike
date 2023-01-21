using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component representing the main controller for an entity.
/// </summary>
public class EntityController : MonoBehaviour
{
    public EntityData EntityState { get; private set; } = new EntityData();

    [SerializeField]
    private float walkSpeed = 1f;
    [SerializeField]
    private float interactionDistance = 0.5f;
    [SerializeField]
    private float attackDuration = 1f;
    [SerializeField]
    private Faction faction;
    [SerializeField]
    private List<Faction> enemyFactions;

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
        InitializeHitbox();
    }

    private void Update()
    {
        UpdateStunTimer();
        if (animatorUpdater != null)
        {
            animatorUpdater.UpdateAnimator(EntityState);
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
        if (IsValidAttackTarget(attackData))
        {
            AttackResult attackResult = damageable.HandleAttack(attackData);
            if (attackResult.IsDead)
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
            range = interactionDistance + attack.AttackType.Range;
        }
        return range;
    }

    /// <summary>
    /// Kills the entity.
    /// </summary>
    private void Die()
    {
        Interrupt();
        Destroy(gameObject);
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
                movement.SetMovement(moveDirection, walkSpeed);

                if (moveDirection != Vector2.zero)
                {
                    EntityState.ActionState = ActionState.Move;
                }
                else
                {
                    EntityState.ActionState = ActionState.Stand;
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
                EntityState.LookDirection = lookDirection;
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
            EntityState.StunTimer = attackDuration;
            EntityState.ActionState = ActionState.Attack;
            if (movement != null)
            {
                movement.SetMovement(Vector2.zero, 0);
            }
            attack.Use(EntityState.LookDirection, interactionDistance, enemyFactions);
        }
    }

    /// <summary>
    /// Determines if the entity is able to act.
    /// </summary>
    /// <returns>true if the entity can act</returns>
    private bool CanAct()
    {
        return EntityState.ActionState != ActionState.Attack
            && EntityState.ActionState != ActionState.Hitstun;
    }

    /// <summary>
    /// If the entity is stunned/attacking, subtract the time in seconds passed from the last
    /// update. If the timer is below 0, tell the entity to change state. Also sets
    /// the move direction to any move direction attempted while the entity was stunned.
    /// </summary>
    private void UpdateStunTimer()
    {
        if (!CanAct())
        {
            EntityState.StunTimer -= Time.deltaTime;
            if (EntityState.StunTimer <= 0)
            {
                EntityState.ActionState = ActionState.Stand;
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
            EntityState.ActionState = ActionState.Hitstun;
            EntityState.StunTimer = attackResult.HitStunDuration;
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
        EntityState.ActionState = ActionState.Idle;
        movement.SetMovement(Vector2.zero, walkSpeed);
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
    /// Determines if the entity is a valid attack target for the attack.
    /// </summary>
    /// <param name="attackData">The AttackData for the attack</param>
    /// <returns>true if the entity is a valid target for the attack</returns>
    private bool IsValidAttackTarget(AttackData attackData)
    {
        return damageable != null
            && gameObject != attackData.User
            && attackData.TargetFactions.Contains(faction);
    }

    /// <summary>
    /// Initializes the entity's hitbox components.
    /// </summary>
    private void InitializeHitbox()
    {
        AttackOnHit attackOnHit = GetComponentInChildren<AttackOnHit>();
        if (attackOnHit != null)
        {
            attackOnHit.attackData.TargetFactions = enemyFactions;
        }
    }
}
