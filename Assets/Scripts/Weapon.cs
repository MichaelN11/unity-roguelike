using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    private void Update()
    {
        Vector2 direction = animatorUpdater.LookDirection.normalized;
        if (animatorUpdater.IsAiming())
        {
            spriteRenderer.enabled = true;
            transform.localPosition = DetermineLocalPosition(direction);
            transform.rotation = UnityUtil.RotateTowardsVector(direction);
            UpdateAttackAnimation();
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
    /// Sets the attack trigger on the Animator, if it hasn't been set during the
    /// current attack state.
    /// </summary>
    private void UpdateAttackAnimation()
    {
        if (entityState.ActionState == ActionState.UsingAbility)
        {
            if (!animatorUpdater.HasAttacked)
            {
                animator.SetTrigger("attack");
            }
        }
    }
}
