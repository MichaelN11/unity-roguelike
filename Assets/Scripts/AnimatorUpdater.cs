using System;
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
    /// <param name="entityData">The entity's state</param>
    public void UpdateAnimator(EntityData entityData)
    {
        if (animator != null)
        {
            UpdateAttack(entityData);
            UpdateLookDirection(entityData);
            UpdateIsMoving(entityData);
            UpdateIsHitstun(entityData);
            UpdateIsIdle(entityData);
            UpdateIsDead(entityData);
        }
    }

    /// <summary>
    /// Determines if the entity is dead and sets IsDead on the Animator.
    /// </summary>
    /// <param name="entityData">The entity's state</param>
    private void UpdateIsDead(EntityData entityData)
    {
        if (entityData.ActionState == ActionState.Dead)
        {
            Debug.Log("trying to play death animation!");
            animator.SetBool("isDead", true);
        } else
        {
            animator.SetBool("isDead", false);
        }
    }

    /// <summary>
    /// Sets the attack trigger on the Animator, if it hasn't been set during the
    /// current attack state.
    /// </summary>
    /// <param name="entityData">The entity's state</param>
    private void UpdateAttack(EntityData entityData)
    {
        if (entityData.ActionState == ActionState.Attack)
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
    /// <param name="entityData">The entity's state</param>
    private void UpdateLookDirection(EntityData entityData)
    {
        if (entityData.LookDirection != null)
        {
            animator.SetFloat("xDirection", entityData.LookDirection.x);
            animator.SetFloat("yDirection", entityData.LookDirection.y);
        }
    }

    /// <summary>
    /// Updates the Animator isMoving property, from the EntityState's Action.
    /// </summary>
    /// <param name="entityData">The entity's state</param>
    private void UpdateIsMoving(EntityData entityData)
    {
        if (entityData.ActionState == ActionState.Move)
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
    /// <param name="entityData">The entity's state</param>
    private void UpdateIsHitstun(EntityData entityData)
    {
        if (entityData.ActionState == ActionState.Hitstun)
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
    /// <param name="entityData"></param>
    private void UpdateIsIdle(EntityData entityData)
    {
        if (entityData.ActionState == ActionState.Idle)
        {
            animator.SetBool("isIdle", true);
        }
        else
        {
            animator.SetBool("isIdle", false);
        }
    }
}
