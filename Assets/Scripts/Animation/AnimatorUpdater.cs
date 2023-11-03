using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component responsible for updating the Animator component.
/// </summary>
public class AnimatorUpdater : MonoBehaviour
{
    public bool IsAiming { get; private set; } = false;
    private float aimModeTimer = 0f;

    private EntityState entityState;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Movement movement;
    private AbilityManager abilityManager;
    private Material defaultMaterial;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        entityState = GetComponent<EntityState>();
        movement = GetComponent<Movement>();
        abilityManager = GetComponentInChildren<AbilityManager>();
    }

    private void Start()
    {
        defaultMaterial = spriteRenderer.material;
        if (abilityManager != null)
        {
            abilityManager.OnAbilityUse += Attack;
        }
    }

    private void Update()
    {
        if (IsAiming)
        { 
            if (aimModeTimer > 0)
            {
                aimModeTimer -= Time.deltaTime;
            }
            else {
                IsAiming = false;
            }
        }

        if (animator != null)
        {
            if (IsAiming)
            {
                UpdateLookDirection();
            }
            UpdateIsMoving();
            UpdateIsHitstun();
            UpdateIsIdle();
            UpdateIsDead();
            UpdateFlash();
            UpdateStop();
            UpdateAttack();
        }
    }

    /// <summary>
    /// Starts aiming, where the sprite looks toward the look direction as opposed to the
    /// direction of movement.
    /// </summary>
    /// <param name="duration"></param>
    private void StartAiming(float duration)
    {
        IsAiming = true;
        aimModeTimer = duration;
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
    /// <param name="eventInfo">Info about the triggered ability use event</param>
    private void Attack(AbilityUseEventInfo eventInfo)
    {
        UpdateLookDirection();
        animator.SetTrigger("attack");
        animator.SetBool("isAttacking", true);
        animator.SetInteger("attackStage", GetAttackStage(eventInfo.AbilityAnimation));

        animator.SetFloat("castSpeed", AnimationUtil.GetAnimationSpeedFromTime(eventInfo.CastTime));
        animator.SetFloat("abilitySpeed", AnimationUtil.GetAnimationSpeedFromTime(eventInfo.ActiveTime));

        animator.SetBool("isActiveAbility", true);
        Invoke(nameof(AbilityNotActive), eventInfo.ActiveTime + eventInfo.CastTime);

        StartAiming(Math.Max(eventInfo.AimDuration, eventInfo.GetFullDuration()));
    }

    /// <summary>
    /// Gets the attack stage for the animator. The attack stage determines which animation to use with the attack for combos.
    /// </summary>
    /// <param name="abilityAnimation">The attack ability animation</param>
    /// <returns>The attack stage as an int</returns>
    private int GetAttackStage(AbilityAnimation abilityAnimation)
    {
        int animationStage = 0;
        switch (abilityAnimation)
        {
            case AbilityAnimation.Slash1:
                animationStage = 1;
                break;
            case AbilityAnimation.Slash2:
                animationStage = 2;
                break;
            case AbilityAnimation.Stab:
                animationStage = 3;
                break;
            case AbilityAnimation.Charge:
                animationStage = 4;
                break;
            case AbilityAnimation.Action:
                animationStage = 5;
                break;
            case AbilityAnimation.Swing1:
                animationStage = 6;
                break;
        }
        return animationStage;
    }

    private void UpdateAttack()
    {
        if (entityState.ActionState != ActionState.Ability)
        {
            animator.SetBool("isAttacking", false);
            animator.SetBool("isActiveAbility", false);
        }
    }

    /// <summary>
    /// Updates the look direction on the Animator, from the EntityState. Only updates
    /// the direction if the entity is in aim mode. Otherwise, look direction is determined
    /// by movement.
    /// </summary>
    private void UpdateLookDirection()
    {
        if (entityState.LookDirection != null)
        {
            SetLookDirection(entityState.LookDirection);
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
            if (!IsAiming && movement != null)
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

    private void AbilityNotActive()
    {
        animator.SetBool("isActiveAbility", false);
    }
}
