using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class responsible for animating an entity.
/// </summary>
public class AnimationController
{
    public Animator Animator { get; set; }

    /// <summary>
    /// Updates the Animator using the passed entity's state.
    /// </summary>
    /// <param name="entityState">The entity's state</param>
    public void UpdateAnimator(EntityState entityState)
    {
        if (Animator != null)
        {
            UpdateLookDirection(entityState);
            UpdateIsMoving(entityState);
        }
    }

    /// <summary>
    /// Sets the attack trigger on the Animator.
    /// </summary>
    public void Attack()
    {
        Animator.SetTrigger("attack");
    }

    /// <summary>
    /// Updates the look direction on the Animator, from the EntityState.
    /// </summary>
    /// <param name="entityState">The entity's state</param>
    private void UpdateLookDirection(EntityState entityState)
    {
        Animator.SetFloat("xDirection", entityState.LookDirection.x);
        Animator.SetFloat("yDirection", entityState.LookDirection.y);
    }

    /// <summary>
    /// Updates the Animator isMoving property, from the EntityState's Action.
    /// </summary>
    /// <param name="entityState">The entity's state</param>
    private void UpdateIsMoving(EntityState entityState)
    {
        if (entityState.Action == Action.Move)
        {
            Animator.SetBool("isMoving", true);
        } else
        {
            Animator.SetBool("isMoving", false);
        }
    }
}
