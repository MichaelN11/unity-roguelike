using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component for controlling a weapon attached to an entity.
/// </summary>
public class Weapon : MonoBehaviour
{
    [SerializeField]
    private float pixelsPerUnit = 16;

    private AnimatorUpdater animatorUpdater;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private EntityState entityState;
    private float distance = 0;
    private float yOffset = 0;
    private bool attacking = false;

    private void Awake()
    {
        animatorUpdater = GetComponentInParent<AnimatorUpdater>();
        entityState = GetComponentInParent<EntityState>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        distance = transform.localPosition.x;
        yOffset = transform.localPosition.y;
        entityState.AbilityEvent += Attack;
        entityState.UnstunnedEvent += StopAttack;
    }

    private void Update()
    {
        if (entityState.ActionState != ActionState.Ability)
        {
            StopAttack();
        }

        Vector2 direction = animatorUpdater.LookDirection.normalized;
        if (animatorUpdater.IsAiming())
        {
            spriteRenderer.enabled = true;
            transform.localPosition = DetermineLocalPosition(direction);
            transform.rotation = UnityUtil.RotateTowardsVector(direction);
        } else
        {
            spriteRenderer.enabled = false;
        }
    }

    /// <summary>
    /// Determines the local position of the object around the parent, using the distance and y offset.
    /// </summary>
    /// <param name="direction">The direction as a Vector2</param>
    /// <returns>The local position as a Vector2</returns>
    private Vector2 DetermineLocalPosition(Vector2 direction)
    {
        Vector2 newLocalPosition = direction * distance;
        newLocalPosition.y += yOffset;
        return UnityUtil.PixelPerfectClamp(newLocalPosition, pixelsPerUnit);
    }

    /// <summary>
    /// Sets the attack trigger on the Animator. Triggered by the attack event.
    /// </summary>
    private void Attack()
    {
        if (!attacking)
        {
            animator.SetTrigger("startAttack");
            attacking = true;
        }
    }

    /// <summary>
    /// Sets the attack to stop on the Animator.
    /// </summary>
    private void StopAttack()
    {
        if (attacking)
        {
            animator.SetTrigger("stopAttack");
            attacking = false;
        }
    }
}
