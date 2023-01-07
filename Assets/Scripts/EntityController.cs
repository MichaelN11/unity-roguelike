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
    private float interactionDistance = 0.5f;
    [SerializeField]
    private float attackDuration = 1f;
    private readonly AnimationController animationController = new();
    private Movement movement;
    private Attack attack;

    private Vector2 attemptedMoveDirection = Vector2.zero;
    private Vector2 attemptedLookDirection = Vector2.zero;

    private void Start()
    {
        movement = GetComponent<Movement>();
        attack = GetComponent<Attack>();
        animationController.Animator = GetComponent<Animator>();
    }

    private void Update()
    {
        animationController.UpdateAnimator(EntityState);
        UpdateAttackTimer();
    }

    /// <summary>
    /// Sets the movement direction for the passed direction vector. Sets the direction
    /// on the movement component, and changes the entity's state to match the new direction.
    /// </summary>
    /// <param name="moveDirection">The new direction to move</param>
    public void SetMovementDirection(Vector2 moveDirection)
    {
        attemptedMoveDirection = moveDirection;
        if (movement != null && CanAct())
        {
            movement.Direction = moveDirection;

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

    /// <summary>
    /// Sets the look direction to the passed vector2.
    /// </summary>
    /// <param name="lookDirection">The new direction to look</param>
    public void SetLookDirection(Vector2 lookDirection)
    {
        attemptedLookDirection = lookDirection;
        if (CanAct())
        {
            EntityState.LookDirection = lookDirection;
        }
    }

    /// <summary>
    /// Tells the entity to attack, if it's able to act.
    /// </summary>
    public void Attack()
    {
        if (attack != null && CanAct())
        {
            animationController.Attack();
            EntityState.AttackTimer = attackDuration;
            EntityState.Action = Action.Attack;
            if (movement != null)
            {
                movement.Direction = Vector2.zero;
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
        return EntityState.Action != Action.Attack;
    }

    /// <summary>
    /// If the entity is attacking, subtract the time in seconds passed from the last
    /// update. If the timer is below 0, tell the entity to stop attacking. Also sets
    /// the move direction to any move direction attempted while the entity was attacking.
    /// </summary>
    private void UpdateAttackTimer()
    {
        if (EntityState.Action == Action.Attack)
        {
            EntityState.AttackTimer -= Time.deltaTime;
            if (EntityState.AttackTimer <= 0)
            {
                EntityState.Action = Action.Stand;
                SetMovementDirection(attemptedMoveDirection);
                SetLookDirection(attemptedLookDirection);
            }
        }
    }
}
