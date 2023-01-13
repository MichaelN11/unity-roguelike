using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component responsible for updating the Animator component.
/// </summary>
public class AnimatorWrapper : MonoBehaviour
{
    private Animator animator;

    private bool hasAttacked = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Updates the Animator using the passed entity's state.
    /// </summary>
    /// <param name="entityState">The entity's state</param>
    public void UpdateAnimator(EntityState entityState)
    {
        if (animator != null)
        {
            UpdateAttack(entityState);
            UpdateLookDirection(entityState);
            UpdateIsMoving(entityState);
            UpdateIsHitstun(entityState);
        }
    }

    /// <summary>
    /// Sets the attack trigger on the Animator, if it hasn't been set during the
    /// current attack state.
    /// </summary>
    /// <param name="entityState">The entity's state</param>
    private void UpdateAttack(EntityState entityState)
    {
        if (entityState.Action == Action.Attack)
        {
            if (!hasAttacked)
            {
                animator.SetTrigger("attack");
                hasAttacked = true;
            }
        } else
        {
            hasAttacked = false;
        }
    }

    /// <summary>
    /// Updates the look direction on the Animator, from the EntityState.
    /// </summary>
    /// <param name="entityState">The entity's state</param>
    private void UpdateLookDirection(EntityState entityState)
    {
        if (entityState.LookDirection != null)
        {
            animator.SetFloat("xDirection", entityState.LookDirection.x);
            animator.SetFloat("yDirection", entityState.LookDirection.y);
        }
    }

    /// <summary>
    /// Updates the Animator isMoving property, from the EntityState's Action.
    /// </summary>
    /// <param name="entityState">The entity's state</param>
    private void UpdateIsMoving(EntityState entityState)
    {
        if (entityState.Action == Action.Move)
        {
            animator.SetBool("isMoving", true);
        } else
        {
            animator.SetBool("isMoving", false);
        }
    }

    /// <summary>
    /// Updates the Animator isHitstun property, from the EntityState's Action.
    /// </summary>
    /// <param name="entityState">The entity's state</param>
    private void UpdateIsHitstun(EntityState entityState)
    {
        if (entityState.Action == Action.Hitstun)
        {
            animator.SetBool("isHitstun", true);
        }
        else
        {
            animator.SetBool("isHitstun", false);
        }
    }
}
