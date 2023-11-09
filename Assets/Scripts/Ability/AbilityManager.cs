using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component for managing an entity's abilities.
/// </summary>
public class AbilityManager : MonoBehaviour
{
    /// <summary>
    /// The event that fires when an ability is used by the entity.
    /// </summary>
    public event Action<AbilityUseEventInfo> OnAbilityUse;

    private List<ActiveAbility> abilities;

    private EntityData entityData;
    private EntityState entityState;
    private Movement movement;
    private Damageable damageable;
    private Hitbox hitbox;
    private SpriteRenderer spriteRenderer;

    private IEnumerator coroutine;

    private ComboAbility currentComboAbility;
    private int nextComboNumber = 0;
    private float comboTimer = 0;
    private float comboableTime = 0;

    private float cancelableDuration = 0;

    private OnUseAbility currentOnUseAbility;
    private EffectData currentAbilityData;
    private float currentAbilityDuration;

    private void Awake()
    {
        entityData = GetComponentInParent<EntityData>();
        entityState = GetComponentInParent<EntityState>();
        movement = GetComponentInParent<Movement>();
        damageable = GetComponentInParent<Damageable>();
        hitbox = entityData.GetComponentInChildren<Hitbox>();
        spriteRenderer = GetComponentInParent<SpriteRenderer>();
    }

    private void Update()
    {
        if (comboTimer > 0)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0)
            {
                ResetCombo();
            }
        }

        if (currentOnUseAbility != null)
        {
            currentAbilityDuration += Time.deltaTime;
        }
    }

    /// <summary>
    /// Creates a new AbilityManager component and adds it to the passed object.
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="abilities"></param>
    /// <returns></returns>
    public static AbilityManager AddToObject(GameObject gameObject, List<ActiveAbility> abilities)
    {
        AbilityManager abilityManager = gameObject.AddComponent<AbilityManager>();
        if (abilities.Count > 0)
        {
            abilityManager.abilities = abilities;
        }
        return abilityManager;
    }

    /// <summary>
    /// Uses the ability.
    /// </summary>
    /// <param name="abilityNumber">The number of the used ability in the hotbar/UI</param>
    /// <param name="direction">The direction in which the ability was used</param>
    /// <param name="offsetDistance">The distance offset from the entity</param>
    /// <returns>AbilityUseEventInfo, or null if unsuccessful</returns>
    public AbilityUseEventInfo UseAbility(int abilityNumber, Vector2 direction, float offsetDistance)
    {
        ActiveAbility ability = GetAbility(abilityNumber);
        return UseAbility(ability, direction, offsetDistance);
    }

    public AbilityUseEventInfo UseAbility(ActiveAbility ability, Vector2 direction, float offsetDistance)
    {
        if (ability is OnUseAbility onUseAbility)
        {
            return UseOnUseAbility(onUseAbility, direction, offsetDistance);
        }
        else if (ability is ComboAbility comboAbility)
        {
            return UseComboAbility(comboAbility, direction, offsetDistance);
        }

        return null;
    }

    /// <summary>
    /// Interrupts the ability.
    /// </summary>
    public void Interrupt()
    {
        ResetCombo();
        StopAllCoroutines();
        InterruptCurrentAbility();
    }

    public List<UsableAbilityInfo> GetUsableAbilities()
    {
        List<UsableAbilityInfo> usableAbilities = new();
        for(int i = 0; i < abilities.Count; i++)
        {
            ActiveAbility ability = abilities[i];
            if (ability is OnUseAbility onUseAbility)
            {
                UsableAbilityInfo usableAbilityInfo = new();
                usableAbilityInfo.Ability = onUseAbility;
                usableAbilityInfo.AbilityNumber = i;
                usableAbilities.Add(usableAbilityInfo);
            } else if (ability is ComboAbility comboAbility)
            {
                int comboStage = 0;
                if (currentComboAbility == comboAbility)
                {
                    comboStage = nextComboNumber;
                }
                UsableAbilityInfo usableAbilityInfo = new();
                usableAbilityInfo.Ability = comboAbility.ComboStages[comboStage].Ability;
                usableAbilityInfo.AbilityNumber = i;
                usableAbilities.Add(usableAbilityInfo);
            }
        }
        return usableAbilities;
    }

    private ActiveAbility GetAbility(int abilityNumber)
    {
        ActiveAbility ability = null;
        if (abilityNumber < abilities.Count)
        {
            ability = abilities[abilityNumber];
        }
        return ability;
    }

    private AbilityUseEventInfo UseOnUseAbility(OnUseAbility onUseAbility, Vector2 direction, float offsetDistance)
    {
        if (entityState.CanAct() || CanCancelInto(onUseAbility))
        {
            AbilityUseEventInfo abilityUseEvent = StartCastingAbility(onUseAbility, direction);
            coroutine = DelayOnUseAbility(onUseAbility, abilityUseEvent.AbilityUse, offsetDistance);
            StartCoroutine(coroutine);
            return abilityUseEvent;
        } else
        {
            return null;
        }
    }

    private bool CanCancelInto(OnUseAbility onUseAbility)
    {
        return entityState.ActionState == ActionState.Ability
            && onUseAbility.CanCancelInto
            && entityState.StunTimer <= cancelableDuration;
    }

    private AbilityUseEventInfo UseComboAbility(ComboAbility comboAbility, Vector2 direction, float offsetDistance)
    {
        if (currentComboAbility != comboAbility)
        {
            ResetCombo();
            currentComboAbility = comboAbility;
        }
        if (entityState.CanAct()
                || (entityState.ActionState == ActionState.Ability
                && entityState.StunTimer <= comboableTime)) {
            comboTimer = 0;
            ComboStage nextComboStage = comboAbility.ComboStages[nextComboNumber];
            AbilityUseEventInfo abilityUseEvent = StartCastingAbility(nextComboStage.Ability, direction);
            coroutine = DelayComboAbility(comboAbility, nextComboStage, abilityUseEvent.AbilityUse, offsetDistance);
            StartCoroutine(coroutine);
            return abilityUseEvent;
        } else
        {
            return null;
        }
    }

    private AbilityUseEventInfo StartCastingAbility(OnUseAbility onUseAbility, Vector2 direction)
    {
        if (onUseAbility.SoundOnCast != null)
        {
            AudioManager.Instance.Play(onUseAbility.SoundOnCast);
        }

        EffectData abilityUse = new()
        {
            Position = transform.position,
            Entity = gameObject,
            EntityData = entityData,
            EntityState = entityState,
            Movement = movement,
            Damageable = damageable,
            Hitbox = hitbox,
            SpriteRenderer = spriteRenderer,
            AbilityManager = this
        };

        if (movement != null)
        {
            movement.StopMoving();
        }

        InterruptCurrentAbility();

        entityState.LookDirection = direction;

        entityState.AbilityState(onUseAbility.RecoveryTime + onUseAbility.CastTime + onUseAbility.ActiveAnimationTime, onUseAbility.AimWhileCasting);
        AbilityUseEventInfo abilityUseEvent = new()
        {
            AbilityUse = abilityUse,
            AbilityAnimation = onUseAbility.AbilityAnimation,
            CastTime = onUseAbility.CastTime,
            ActiveTime = onUseAbility.ActiveAnimationTime,
            RecoveryTime = onUseAbility.RecoveryTime,
            AimDuration = onUseAbility.AimDuration,
            Range = onUseAbility.Range,
            ChangeDirection = onUseAbility.ChangeDirection
        };
        OnAbilityUse?.Invoke(abilityUseEvent);
        cancelableDuration = onUseAbility.CancelableDuration;

        return abilityUseEvent;
    }

    private void ResetCombo()
    {
        nextComboNumber = 0;
        comboTimer = 0;
        comboableTime = 0;
    }

    /// <summary>
    /// Coroutine method that delays the on use ability's start time. Used for cast or startup times.
    /// </summary>
    /// <param name="onUseAbility"></param>
    /// <param name="abilityUse"></param>
    /// <param name="offsetDistance"></param>
    /// <returns>IEnumerator used for the coroutine</returns>
    private IEnumerator DelayOnUseAbility(OnUseAbility onUseAbility, EffectData abilityUse, float offsetDistance)
    {
        yield return new WaitForSeconds(onUseAbility.CastTime);
        UpdateAbilityState(onUseAbility, abilityUse, offsetDistance);
        onUseAbility.Use(abilityUse);
    }

    private IEnumerator DelayComboAbility(ComboAbility comboAbility, ComboStage nextComboStage, EffectData abilityUse, float offsetDistance)
    {
        yield return new WaitForSeconds(nextComboStage.Ability.CastTime);
        UpdateAbilityState(nextComboStage.Ability, abilityUse, offsetDistance);
        nextComboStage.Ability.Use(abilityUse);

        if (nextComboNumber + 1 < comboAbility.ComboStages.Count)
        {
            comboTimer = nextComboStage.ComboContinueWindow;
            comboableTime = nextComboStage.ComboCancelableDuration;
            ++nextComboNumber;
        }
        else
        {
            ResetCombo();
        }
    }

    private void UpdateAbilityState(OnUseAbility onUseAbility, EffectData abilityUse, float offsetDistance)
    {
        entityState.CanLookWhileCasting = false;
        abilityUse.Direction = entityState.LookDirection.normalized;
        abilityUse.Position += entityState.LookDirection.normalized * offsetDistance;

        currentOnUseAbility = onUseAbility;
        currentAbilityData = abilityUse;
        currentAbilityDuration = 0;
    }

    private void InterruptCurrentAbility()
    {
        if (currentOnUseAbility != null && currentAbilityData != null)
        {
            currentOnUseAbility.Interrupt(currentAbilityData, currentAbilityDuration);
            currentOnUseAbility = null;
        }
    }
}