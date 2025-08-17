using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component for controlling a weapon attached to an entity.
/// </summary>
public class WeaponController : MonoBehaviour
{
    private const float SwingRotationDegrees = 110; 

    [SerializeField]
    private float pixelsPerUnit = 16;

    private Weapon weapon;
    private AnimatorUpdater animatorUpdater;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private AbilityManager abilityManager;
    private EntityState entityState;
    private float distance = 0;
    private float yOffset = 0;
    private Vector2 pivot = Vector2.zero;
    private Vector2 initialLocalScale;

    private AbilityAnimation currentAnimation = AbilityAnimation.Default;
    private float currentAbilityRange = 0;

    private Vector2 swingPivot;
    private float swingTotalTime = 0;
    private float swingAnimationTimer = 0;
    private float swingRecoveryTimer = 0;

    private void Awake()
    {
        abilityManager = UnityUtil.GetParentIfExists(gameObject).GetComponentInChildren<AbilityManager>();
        animatorUpdater = GetComponentInParent<AnimatorUpdater>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        entityState = GetComponentInParent<EntityState>();
    }

    private void Start()
    {
        Transform pivotTransform = transform.parent.Find("WeaponPivot");
        if (pivotTransform != null)
        {
            pivot = pivotTransform.localPosition - abilityManager.transform.localPosition;
        }

        distance = transform.localPosition.x;
        yOffset = transform.localPosition.y;

        distance -= pivot.x;
        yOffset -= pivot.y;

        initialLocalScale = transform.localScale;

        if (abilityManager != null)
        {
            abilityManager.OnAbilityStarted += AbilityStarted;
            abilityManager.OnAbilityUse += AbilityUse;
        }

        swingPivot =  new Vector2(0, spriteRenderer.bounds.extents.y); ;
    }

    private void Update()
    {
        if (animatorUpdater.IsAiming
            && entityState.ActionState != ActionState.Dead
            && IsWeaponDisplayable())
        {
            ShowWeapon();
            SwingWeapon();
        } else
        {
            spriteRenderer.enabled = false;
        }
    }

    /// <summary>
    /// Creates a new WeaponController component and adds it to the passed object.
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="weapon"></param>
    /// <returns></returns>
    public static WeaponController AddToObject(GameObject gameObject, Weapon weapon)
    {
        WeaponController weaponController = gameObject.AddComponent<WeaponController>();
        weaponController.weapon = weapon;
        return weaponController;
    }

    private void ShowWeapon()
    {
        RotateAndPositionWeapon();
        spriteRenderer.enabled = true;
    }

    private void RotateAndPositionWeapon()
    {
        Vector2 lookDirection = entityState.LookDirection.normalized;
        bool mirrorInXDirection = weapon.MirrorXDirection && lookDirection.x < 0;
        Vector2 directionalPivot = GetDirectionalPivot(mirrorInXDirection);
        Vector2 direction = DetermineDirection(lookDirection, directionalPivot);
        direction = HandleMirroredDirection(direction, mirrorInXDirection);
        transform.localPosition = DetermineLocalPosition(direction, directionalPivot, mirrorInXDirection);
        transform.rotation = UnityUtil.RotateTowardsVector(direction);
    }

    private void SwingWeapon()
    {
        if (swingAnimationTimer > 0 || swingRecoveryTimer > 0)
        {
            float t = 1 - (swingAnimationTimer / swingTotalTime);
            float rotation = Mathf.Lerp(0, SwingRotationDegrees, t);
            Vector3 pivotPoint = transform.TransformPoint(swingPivot);
            transform.RotateAround(pivotPoint, Vector3.forward, rotation);

            if (swingAnimationTimer > 0)
            {
                swingAnimationTimer -= Time.deltaTime;
            }
            else if (swingRecoveryTimer > 0)
            {
                swingRecoveryTimer -= Time.deltaTime;
            }
        }
    }

    private bool IsWeaponDisplayable()
    {
        return weapon.DisplayAnimations.Count == 0
            || weapon.DisplayAnimations.Contains(currentAnimation);
    }

    /// <summary>
    /// Gets the pivot accounting for the direction the entity is facing in.
    /// </summary>
    /// <param name="mirrorInXDirection">If the pivot should be mirrored in the X direction</param>
    /// <returns>The pivot accounting for the direction</returns>
    private Vector2 GetDirectionalPivot(bool mirrorInXDirection)
    {
        Vector2 directionalPivot = pivot;
        if (mirrorInXDirection)
        {
            directionalPivot.x *= -1;
        }
        return directionalPivot;
    }

    private Vector2 DetermineDirection(Vector2 lookDirection, Vector2 directionalPivot)
    {
        if (directionalPivot == Vector2.zero)
        {
            return lookDirection;
        }
        Vector2 position = abilityManager.transform.position;
        Vector2 attackPosition = position + (lookDirection * currentAbilityRange);
        Vector2 direction = attackPosition - (directionalPivot + position);
        return direction.normalized;
    }

    /// <summary>
    /// Handles for weapons that need to be mirrored in a direction.
    /// </summary>
    /// <param name="direction">The direction the weapon is going to face</param>
    /// <param name="mirrorInXDirection">If the x direction should be mirrored</param>
    /// <returns></returns>
    private Vector2 HandleMirroredDirection(Vector2 direction, bool mirrorInXDirection)
    {
        if (mirrorInXDirection)
        {
            direction.x *= -1;
            direction.y *= -1;
            transform.localScale = new Vector2(initialLocalScale.x * -1, initialLocalScale.y);
        } else
        {
            transform.localScale = initialLocalScale;
        }
        return direction;
    }

    /// <summary>
    /// Determines the local position of the object around the parent, using the distance and y offset.
    /// </summary>
    /// <param name="direction">The direction as a Vector2</param>
    /// <param name="directionalPivot">The pivot for the object</param>
    /// <param name="mirrorInXDirection">If the X direction should be mirrored</param>
    /// <returns>The local position as a Vector2</returns>
    private Vector2 DetermineLocalPosition(Vector2 direction, Vector2 directionalPivot, bool mirrorInXDirection)
    {
        float distanceFromPosition = distance;
        if (mirrorInXDirection)
        {
            distanceFromPosition *= -1;
        }
        Vector2 newLocalPosition = (direction * distanceFromPosition) + directionalPivot;
        newLocalPosition.y += yOffset;
        return UnityUtil.PixelPerfectClamp(newLocalPosition, pixelsPerUnit);
    }

    /// <summary>
    /// Sets the attack trigger on the Animator. Triggered by the ability started event.
    /// </summary>
    /// <param name="eventInfo">The ability use event data</param>
    private void AbilityStarted(AbilityUseEventInfo eventInfo)
    {
        currentAnimation = eventInfo.AbilityAnimation;
        currentAbilityRange = eventInfo.Range;
        if (!weapon.SwingRotateAnimations.Contains(currentAnimation))
        {
            UpdateAnimator(eventInfo);
        }        
    }

    private void AbilityUse(AbilityUseEventInfo eventInfo)
    {
        if (weapon.SwingRotateAnimations.Contains(currentAnimation))
        {
            swingAnimationTimer = eventInfo.ActiveTime;
            swingRecoveryTimer = eventInfo.RecoveryTime;
            swingTotalTime = swingAnimationTimer;
        }
    }

    private void UpdateAnimator(AbilityUseEventInfo eventInfo)
    {
        animator.SetTrigger("attack");

        float castSpeed = -1;
        float abilitySpeed = -1;
        if (eventInfo.CastTime > 0)
        {
            castSpeed = 1 / eventInfo.CastTime;
        }
        float totalAbilityTime = eventInfo.ActiveTime + eventInfo.RecoveryTime;
        if (totalAbilityTime > 0)
        {
            abilitySpeed = 1 / totalAbilityTime;
        }
        animator.SetFloat("castSpeed", castSpeed);
        animator.SetFloat("abilitySpeed", abilitySpeed);
    }
}
