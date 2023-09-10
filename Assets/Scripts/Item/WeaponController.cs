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
        distance = transform.localPosition.x;
        yOffset = transform.localPosition.y;
        abilityManager.OnAbilityUse += Attack;
        animator.speed = weapon.AnimationSpeed;
    }

    private void Update()
    {
        Vector2 direction = entityState.LookDirection.normalized;
        if (animatorUpdater.IsAiming() && entityState.ActionState != ActionState.Dead)
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
    /// <param name="eventInfo">The ability use event data</param>
    private void Attack(AbilityUseEventInfo eventInfo)
    {
        animator.SetTrigger("attack");
    }
}
