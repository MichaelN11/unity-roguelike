using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component representing the main controller for an entity.
/// </summary>
[RequireComponent(typeof(EntityState), typeof(EntityData))]
public class EntityController : MonoBehaviour
{
    private Movement movement;
    private AbilityManager abilityManager;
    private Inventory inventory;
    private EntityState entityState;
    private EntityData entityData;

    private Vector2 attemptedMoveDirection = Vector2.zero;
    private Vector2 attemptedLookDirection = Vector2.zero;

    private void Awake()
    {
        movement = GetComponent<Movement>();
        abilityManager = GetComponentInChildren<AbilityManager>();
        entityState = GetComponent<EntityState>();
        entityData = GetComponent<EntityData>();
        inventory = GetComponent<Inventory>();
    }

    private void Start()
    {
        entityState.OnUnstunned += Unstunned;
    }

    /// <summary>
    /// Updates the entity based on the passed InputData.
    /// </summary>
    /// <param name="inputData">The input data containing the input type</param>
    /// <returns>true if the update was successful</returns>
    public bool UpdateFromInput(InputData inputData)
    {
        if (GameManager.Instance.IsPaused)
        {
            return true;
        }

        bool updateSuccessful = false;
        switch (inputData.Type)
        {
            case InputType.Look:
                updateSuccessful = SetLookDirection(inputData.Direction);
                break;
            case InputType.Move:
                updateSuccessful = SetMovementDirection(inputData.Direction);
                break;
            case InputType.Ability:
                updateSuccessful = Ability(inputData.Number, inputData.Direction);
                break;
            case InputType.Item:
                updateSuccessful = Item(inputData.Number, inputData.Direction);
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
                movement.SetMovement(moveDirection, entityData.Entity.WalkSpeed);

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
            if (entityState.CanAct()
                || (entityState.ActionState == ActionState.Ability && entityState.CanLookWhileCasting))
            {
                entityState.LookDirection = lookDirection;
                isLookDirectionSet = true;
            }
        }
        return isLookDirectionSet;
    }

    private bool Ability(int abilityNumber, Vector2 abilityDirection)
    {
        bool successful = false;
        if (abilityManager != null)
        {
            successful = abilityManager.UseAbility(abilityNumber, abilityDirection, entityData.Entity.InteractionDistance);
        }
        return successful;
    }

    private bool Item(int itemNumber, Vector2 direction)
    {
        bool successful = false;
        if (inventory != null)
        {
            successful = inventory.UseItem(itemNumber, direction, entityData.Entity.InteractionDistance);
        }
        return successful;
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
            movement.SetMovement(Vector2.zero, entityData.Entity.WalkSpeed);
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
