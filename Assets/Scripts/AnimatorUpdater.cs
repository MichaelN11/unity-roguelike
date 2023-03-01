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
    public Vector2 LookDirection { get; set; } = Vector2.zero;
    public AttackAnimation AttackAnimation { get; set; } = AttackAnimation.Default;

    [SerializeField]
    private float aimModeDuration = 3f;

    private EntityState entityState;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Movement movement;
    private Material defaultMaterial;
    private float aimModeTimer = 0f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        entityState = GetComponent<EntityState>();
        movement = GetComponent<Movement>();
        defaultMaterial = spriteRenderer.material;
    }

    private void Update()
    {
        if (aimModeTimer > 0)
        {
            aimModeTimer -= Time.deltaTime;
        }

        if (animator != null)
        {
            UpdateAttack();
            UpdateLookDirection();
            UpdateIsMoving();
            UpdateIsHitstun();
            UpdateIsIdle();
            UpdateIsDead();
            UpdateFlash();
            UpdateStop();
        }
    }

    /// <summary>
    /// Determines if the entity is in aim mode.
    /// </summary>
    /// <returns>true if the entity is aiming</returns>
    public bool IsAiming()
    {
        return aimModeTimer > 0;
    }

    /// <summary>
    /// Determines if the entity is dead and sets IsDead on the Animator.
    /// </summary>
    private void UpdateIsDead()
    {
        if (entityState.ActionState == ActionState.Dead)
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
    private void UpdateAttack()
    {
        if (entityState.ActionState == ActionState.UsingAbility)
        {
            if (!HasAttacked)
            {
                animator.SetTrigger("attack");
                animator.SetInteger("attackAnimation", (int) AttackAnimation);
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
    private void UpdateLookDirection()
    {
        if (IsAiming() && LookDirection != null)
        {
            SetLookDirection(LookDirection);
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
    private void UpdateIsMoving()
    {
        if (entityState.ActionState == ActionState.Move)
        {
            animator.SetBool("isMoving", true);
            if (!IsAiming() && movement != null)
            {
                SetLookDirection(movement.Direction);
            }
        } else
        {
            animator.SetBool("isMoving", false);
        }
    }

    /// <summary>
    /// Updates the Animator isHitstun property, from the EntityState's Action.
    /// </summary>
    private void UpdateIsHitstun()
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
    private void UpdateIsIdle()
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

    /// <summary>
    /// Updates the sprites material if the entity is flashing.
    /// </summary>
    private void UpdateFlash()
    {
        if (entityState.IsFlashing())
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
    private void UpdateStop()
    {
        if (entityState.IsStopped())
        {
            animator.speed = 0;
        } else
        {
            animator.speed = 1;
        }
    }
}
