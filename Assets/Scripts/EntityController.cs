using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component representing the main controller for an entity.
/// </summary>
public class EntityController : MonoBehaviour
{
    public EntityState EntityState { get; private set; } = new EntityState();

    private readonly AnimationController animationController = new();

    private Movement movement;

    private void Start()
    {
        movement = GetComponent<Movement>();
        animationController.Animator = GetComponent<Animator>();
    }

    private void Update()
    {
        animationController.UpdateAnimator(EntityState);
    }

    /// <summary>
    /// Sets the movement direction for the passed direction vector. Sets the direction
    /// on the movement component, and changes the entity's state to match the new direction.
    /// </summary>
    /// <param name="moveDirection">The new direction to move</param>
    public void SetMovementDirection(Vector2 moveDirection)
    {
        if (movement != null)
        {
            movement.Direction = moveDirection;
        }

        if (moveDirection != Vector2.zero)
        {
            EntityState.Action = Action.Move;
        } else
        {
            EntityState.Action = Action.Stand;
        }
    }

    /// <summary>
    /// Sets the look direction to the passed vector2.
    /// </summary>
    /// <param name="lookDirection">The new direction to look</param>
    public void SetLookDirection(Vector2 lookDirection)
    {
        EntityState.LookDirection = lookDirection;
    }
}
