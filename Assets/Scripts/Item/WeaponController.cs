using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component for controlling a weapon attached to an entity.
/// </summary>
public class WeaponController : MonoBehaviour
{
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
    private Vector2 pivot;

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
            Debug.Log("pivot = " + pivot);
        }

        distance = transform.localPosition.x;
        yOffset = transform.localPosition.y;
        if (pivot != null)
        {
            distance -= pivot.x;
            yOffset -= pivot.y;
        }

        abilityManager.OnAbilityUse += Attack;
    }

    private void Update()
    {
        if (animatorUpdater.IsAiming && entityState.ActionState != ActionState.Dead)
        {
            Vector2 direction = DetermineDirection();
            spriteRenderer.enabled = true;
            transform.localPosition = DetermineLocalPosition(direction);
            transform.rotation = UnityUtil.RotateTowardsVector(direction);
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

    private Vector2 DetermineDirection()
    {
        Vector2 lookDirection = entityState.LookDirection.normalized;
        if (pivot == null)
        {
            return lookDirection;
        }
        Vector2 position = abilityManager.transform.position;
        Debug.Log("position: " + position + " look dir: " + lookDirection + " range: " + abilityManager.GetRange()
            + " look dir * range: " + (lookDirection * abilityManager.GetRange()));
        Vector2 attackPosition = position + (lookDirection * abilityManager.GetRange());
        Vector2 direction = attackPosition - (pivot + position);
        Debug.Log("position: " + position + " attack pos: " + attackPosition + " direction: " + direction);
        return direction;
    }

    /// <summary>
    /// Determines the local position of the object around the parent, using the distance and y offset.
    /// </summary>
    /// <param name="direction">The direction as a Vector2</param>
    /// <returns>The local position as a Vector2</returns>
    private Vector2 DetermineLocalPosition(Vector2 direction)
    {
        Vector2 newLocalPosition = (direction * distance) + pivot;
        newLocalPosition.y += yOffset;
        return UnityUtil.PixelPerfectClamp(newLocalPosition, pixelsPerUnit);
    }

    /// <summary>
    /// Sets the attack trigger on the Animator. Triggered by the attack event.
    /// </summary>
    /// <param name="eventInfo">The ability use event data</param>
    private void Attack(AbilityUseEventInfo eventInfo)
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
