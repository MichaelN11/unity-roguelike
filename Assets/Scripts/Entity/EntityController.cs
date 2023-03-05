using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component representing the main controller for an entity.
/// </summary>
[RequireComponent(typeof(EntityState), typeof(EntityData))]
public class EntityController : MonoBehaviour
{
    private AnimatorUpdater animatorUpdater;
    private Movement movement;
    private AbilityManager abilityManager;
    private EntityState entityState;
    private EntityData entityData;

    private Vector2 attemptedMoveDirection = Vector2.zero;
    private Vector2 attemptedLookDirection = Vector2.zero;

    private void Awake()
    {
        movement = GetComponent<Movement>();
        abilityManager = GetComponentInChildren<AbilityManager>();
        animatorUpdater = GetComponent<AnimatorUpdater>();
        entityState = GetComponent<EntityState>();
        entityData = GetComponent<EntityData>();
    }

    private void Start()
    {
        entityState.UnstunnedEvent += Unstunned;
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
    /// Gets the attack range for the entity, with the interaction distance and radius added.
    /// TODO This will later return info about the entity's abilities for the AI.
    /// </summary>
    /// <returns>The attack range as a float</returns>
    public float GetAttackRange()
    {
        float range = 0;
        if (abilityManager != null)
        {
            range = entityData.EntityType.InteractionDistance + abilityManager.GetRange();
        }
        return range;
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
            if (movement != null && entityState.CanAct())
            {
                movement.SetMovement(moveDirection, entityData.EntityType.WalkSpeed);

                if (moveDirection != Vector2.zero)
                {
                    entityState.MoveState();
                }
                else
                {
                    entityState.StandState();
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
            if (entityState.CanAct())
            {
                entityState.LookDirection = lookDirection;
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
            Vector2 positionOffset = attackDirection.normalized * entityData.EntityType.InteractionDistance;
            attackSuccessful = abilityManager.UseAbility(attackDirection, positionOffset);
        }
        return attackSuccessful;
    }

    /// <summary>
    /// Makes the entity go idle.
    /// </summary>
    /// <returns>true if the idle action was successful</returns>
    private bool Idle()
    {
        bool isIdleSet = false;
        if (entityState.CanAct())
        {
            entityState.IdleState();
            movement.SetMovement(Vector2.zero, entityData.EntityType.WalkSpeed);
            isIdleSet = true;
        }
        return isIdleSet;
    }

    /// <summary>
    /// Sets the movement and look direction when the entity is unstunned.
    /// </summary>
    private void Unstunned()
    {
        SetMovementDirection(attemptedMoveDirection);
        SetLookDirection(attemptedLookDirection);
    }
}
