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

    private List<Ability> abilities;

    private EntityData entityData;
    private EntityState entityState;
    private Movement movement;
    private Damageable damageable;

    private IEnumerator coroutine;

    private ComboAbility currentComboAbility;
    private int nextComboNumber = 0;
    private float comboTimer = 0;
    private float comboableTime = 0;

    private float cancelableDuration = 0;

    private void Awake()
    {
        entityData = GetComponentInParent<EntityData>();
        entityState = GetComponentInParent<EntityState>();
        movement = GetComponentInParent<Movement>();
        damageable = GetComponentInParent<Damageable>();
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
    }

    /// <summary>
    /// Creates a new AbilityManager component and adds it to the passed object.
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="abilities"></param>
    /// <returns></returns>
    public static AbilityManager AddToObject(GameObject gameObject, List<Ability> abilities)
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
    /// <param name="positionOffset">The position offset from the entity</param>
    /// <returns>true if the ability was used successfully</returns>
    public bool UseAbility(int abilityNumber, Vector2 direction, Vector2 positionOffset)
    {
        Ability ability = GetAbility(abilityNumber - 1);
        if (ability is OnUseAbility onUseAbility)
        {
            return UseOnUseAbility(onUseAbility, direction, positionOffset);
        }
        else if (ability is ComboAbility comboAbility)
        {
            return UseComboAbility(comboAbility, direction, positionOffset);
        }
  
        return false;
    }

    /// <summary>
    /// Interrupts the ability.
    /// </summary>
    public void Interrupt()
    {
        ResetCombo();
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
    }

    /// <summary>
    /// Gets the ability range for the AI. Will need to be updated in the future to return more data.
    /// </summary>
    /// <returns>The ability range as a float</returns>
    public float GetRange()
    {
        Ability ability = GetAbility(0);
        if (ability is OnUseAbility onUseAbility)
        {
            return onUseAbility.Range;
        }
        return 0;
    }

    private Ability GetAbility(int abilityNumber)
    {
        Ability ability = null;
        if (abilityNumber < abilities.Count)
        {
            ability = abilities[abilityNumber];
        }
        return ability;
    }

    private bool UseOnUseAbility(OnUseAbility onUseAbility, Vector2 direction, Vector2 positionOffset)
    {
        if (entityState.CanAct() || CanCancelInto(onUseAbility))
        {
            EffectData abilityUse = StartCastingAbility(onUseAbility, direction, positionOffset);
            coroutine = DelayOnUseAbility(onUseAbility, abilityUse);
            StartCoroutine(coroutine);
            return true;
        } else
        {
            return false;
        }
    }

    private bool CanCancelInto(OnUseAbility onUseAbility)
    {
        return entityState.ActionState == ActionState.Ability
            && onUseAbility.CanCancelInto
            && entityState.StunTimer <= cancelableDuration;
    }

    private bool UseComboAbility(ComboAbility comboAbility, Vector2 direction, Vector2 positionOffset)
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
            EffectData abilityUse = StartCastingAbility(nextComboStage.Ability, direction, positionOffset);
            coroutine = DelayComboAbility(comboAbility, nextComboStage, abilityUse);
            StartCoroutine(coroutine);
            return true;
        } else
        {
            return false;
        }
    }

    private EffectData StartCastingAbility(OnUseAbility onUseAbility, Vector2 direction, Vector2 positionOffset)
    {
        EffectData abilityUse = new()
        {
            Direction = direction,
            Position = transform.position + (Vector3)positionOffset,
            Entity = gameObject,
            EntityData = entityData,
            EntityState = entityState,
            EntityMovement = movement,
            EntityDamageable = damageable
        };

        if (movement != null)
        {
            movement.StopMoving();
        }

        entityState.LookDirection = direction;
        entityState.AbilityState(onUseAbility.RecoveryTime + onUseAbility.CastTime + onUseAbility.ActiveTime);
        OnAbilityUse?.Invoke(new AbilityUseEventInfo()
        {
            AbilityUse = abilityUse,
            AbilityAnimation = onUseAbility.AbilityAnimation,
            CastTime = onUseAbility.CastTime,
            ActiveTime = onUseAbility.ActiveTime,
            RecoveryTime = onUseAbility.RecoveryTime,
            AimDuration = onUseAbility.AimDuration,
            Range = onUseAbility.Range
        });
        cancelableDuration = onUseAbility.CancelableDuration;

        return abilityUse;
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
    /// <returns>IEnumerator used for the coroutine</returns>
    private IEnumerator DelayOnUseAbility(OnUseAbility onUseAbility, EffectData abilityUse)
    {
        yield return new WaitForSeconds(onUseAbility.CastTime);
        onUseAbility.Use(abilityUse);
    }

    private IEnumerator DelayComboAbility(ComboAbility comboAbility, ComboStage nextComboStage, EffectData abilityUse)
    {
        yield return new WaitForSeconds(nextComboStage.Ability.CastTime);
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
}