using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component responsible for updating the Animator component.
/// </summary>
public class AnimatorUpdater : MonoBehaviour
{
    public bool HasAttacked { get; set; } = false;

    [SerializeField]
    private float aimModeDuration = 3f;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Material defaultMaterial;
    private float aimModeTimer = 0f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultMaterial = spriteRenderer.material;
    }

    private void Update()
    {
        if (aimModeTimer > 0)
        {
            aimModeTimer -= Time.deltaTime;
        }
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
            UpdateFlash(entityData);
            UpdateStop(entityData);
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
            animator.SetBool("isDead", true);
        } else
        {
            animator.SetBool("isDead", false);
        }
    }

    /// <summary>
    /// Sets the attack trigger on the Animator, if it hasn't been set during the
    /// current attack state. Also sets the attack animation int that corresponds to the
    /// value in the AttackAnimation enum, so that the animator can determine which attack
    /// animation to use.
    /// </summary>
    /// <param name="entityData">The entity's state</param>
    private void UpdateAttack(EntityData entityData)
    {
        if (entityData.ActionState == ActionState.UsingAbility)
        {
            if (!HasAttacked)
            {
                animator.SetTrigger("attack");
                animator.SetInteger("attackAnimation", (int) entityData.AttackAnimation);
                HasAttacked = true;
                aimModeTimer = aimModeDuration;
            }
        } else
        {
            HasAttacked = false;
        }
    }

    /// <summary>
    /// Updates the look direction on the Animator, from the EntityState. Only updates
    /// the direction if the entity is in aim mode. Otherwise, look direction is determined
    /// by movement.
    /// </summary>
    /// <param name="entityData">The entity's state</param>
    private void UpdateLookDirection(EntityData entityData)
    {
        if (IsAiming() && entityData.LookDirection != null)
        {
            SetLookDirection(entityData.LookDirection);
        }
    }

    /// <summary>
    /// Sets the animator's look direction to the passed Vector2.
    /// </summary>
    /// <param name="direction">The look direction</param>
    private void SetLookDirection(Vector2 direction)
    {
        animator.SetFloat("xDirection", direction.x);
        animator.SetFloat("yDirection", direction.y);
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
            if (!IsAiming())
            {
                SetLookDirection(entityData.MoveDirection);
            }
        } else
        {
            animator.SetBool("isMoving", false);
        }
    }

    /// <summary>
    /// Determines if the entity is in aim mode.
    /// </summary>
    /// <returns>true if the entity is aiming</returns>
    private bool IsAiming()
    {
        return aimModeTimer > 0;
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
    /// <param name="entityData">The entity's state</param>
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

    /// <summary>
    /// Updates the sprites material if the entity is flashing.
    /// </summary>
    /// <param name="entityData">The entity's state</param>
    private void UpdateFlash(EntityData entityData)
    {
        if (entityData.IsFlashing())
        {
            spriteRenderer.material = ResourceManager.Instance.FlashMaterial;
        } else
        {
            spriteRenderer.material = defaultMaterial;
        }
    }

    /// <summary>
    /// Sets the animator speed to 0 when the entity is stopped.
    /// </summary>
    /// <param name="entityData">The entity's state</param>
    private void UpdateStop(EntityData entityData)
    {
        if (entityData.IsStopped())
        {
            animator.speed = 0;
        } else
        {
            animator.speed = 1;
        }
    }
}
