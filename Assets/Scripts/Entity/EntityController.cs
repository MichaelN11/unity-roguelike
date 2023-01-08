using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component representing the main controller for an entity.
/// </summary>
public class EntityController : MonoBehaviour
{
    public EntityState EntityState { get; private set; } = new EntityState();

    [SerializeField]
    private float walkSpeed = 1f;
    [SerializeField]
    private float interactionDistance = 0.5f;
    [SerializeField]
    private float attackDuration = 1f;

    private AnimatorWrapper animatorWrapper;
    private Attack attack;
    private Movement movement;
    private Damageable damageable;

    private Vector2 attemptedMoveDirection = Vector2.zero;
    private Vector2 attemptedLookDirection = Vector2.zero;

    private void Start()
    {
        movement = GetComponent<Movement>();
        attack = GetComponent<Attack>();
        animatorWrapper = GetComponent<AnimatorWrapper>();
        damageable = GetComponent<Damageable>();
    }

    private void Update()
    {
        if (animatorWrapper != null)
        {
            animatorWrapper.UpdateAnimator(EntityState);
        }
        UpdateStunTimer();
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
            default:
                // Unrecognized input
                break;
        }
    }

    /// <summary>
    /// Handles being hit by an incoming attack.
    /// </summary>
    /// <param name="attackData">The attack data</param>
    public void HandleAttack(AttackData attackData)
    {
        if (damageable != null)
        {
            AttackResults attackResults = damageable.HandleAttack(attackData);
            if (attackResults.IsDead)
            {
                Die();
            } else
            {
                HandleHitstun(attackResults);
            }
        }
    }

    /// <summary>
    /// Kills the entity.
    /// </summary>
    private void Die()
    {
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
                movement.UpdateMovement(moveDirection, walkSpeed);

                if (moveDirection != Vector2.zero)
                {
                    EntityState.Action = Action.Move;
                }
                else
                {
                    EntityState.Action = Action.Stand;
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
            EntityState.Action = Action.Attack;
            if (movement != null)
            {
                movement.UpdateMovement(Vector2.zero, 0);
            }
            attack.Use(EntityState.LookDirection, interactionDistance);
        }
    }

    /// <summary>
    /// Determines if the entity is able to act.
    /// </summary>
    /// <returns>true if the entity can act</returns>
    private bool CanAct()
    {
        return EntityState.Action != Action.Attack
            && EntityState.Action != Action.Hitstun;
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
                EntityState.Action = Action.Stand;
                SetMovementDirection(attemptedMoveDirection);
                SetLookDirection(attemptedLookDirection);
            }
        }
    }

    /// <summary>
    /// Handles the hitstun and knockback after being hit by an attack.
    /// </summary>
    /// <param name="attackResults">The results of the attack</param>
    private void HandleHitstun(AttackResults attackResults)
    {
        if (attackResults.HitStunDuration > 0)
        {
            EntityState.Action = Action.Hitstun;
            EntityState.StunTimer = attackResults.HitStunDuration;
            if (movement != null)
            {
                Debug.Log("knockback direction: " + attackResults.KnockbackDirection + " speed: " +
                    attackResults.KnockbackSpeed);
                movement.UpdateMovement(attackResults.KnockbackDirection,
                    attackResults.KnockbackSpeed);
            }
        }
    }
}
