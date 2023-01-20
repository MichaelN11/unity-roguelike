using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component responsible for updating the Animator component.
/// </summary>
public class AnimatorUpdater : MonoBehaviour
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
    public void UpdateAnimator(EntityStateData entityState)
    {
        if (animator != null)
        {
            UpdateAttack(entityState);
            UpdateLookDirection(entityState);
            UpdateIsMoving(entityState);
            UpdateIsHitstun(entityState);
            UpdateIsIdle(entityState);
        }
    }

    /// <summary>
    /// Sets the attack trigger on the Animator, if it hasn't been set during the
    /// current attack state.
    /// </summary>
    /// <param name="entityState">The entity's state</param>
    private void UpdateAttack(EntityStateData entityState)
    {
        if (entityState.ActionState == ActionState.Attack)
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
    private void UpdateLookDirection(EntityStateData entityState)
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
    private void UpdateIsMoving(EntityStateData entityState)
    {
        if (entityState.ActionState == ActionState.Move)
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
    private void UpdateIsHitstun(EntityStateData entityState)
    {
        if (entityState.ActionState == ActionState.Hitstun)
        {
            animator.SetBool("isHitstun", true);
        }
        else
        {
            animator.SetBool("isHitstun", false);
        }
    }

    /// <summary>
    /// Updates the Animator isIdle property, from the EntityState's Action.
    /// </summary>
    /// <param name="entityState"></param>
    private void UpdateIsIdle(EntityStateData entityState)
    {
        if (entityState.ActionState == ActionState.Idle)
        {
            animator.SetBool("isIdle", true);
        }
        else
        {
            animator.SetBool("isIdle", false);
        }
    }
}
