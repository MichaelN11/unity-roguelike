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
    private Hitbox hitbox;

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
        hitbox = entityData.GetComponentInChildren<Hitbox>();
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
    /// <param name="offsetDistance">The distance offset from the entity</param>
    /// <returns>true if the ability was used successfully</returns>
    public bool UseAbility(int abilityNumber, Vector2 direction, float offsetDistance)
    {
        Ability ability = GetAbility(abilityNumber);
        if (ability is OnUseAbility onUseAbility)
        {
            return UseOnUseAbility(onUseAbility, direction, offsetDistance);
        }
        else if (ability is ComboAbility comboAbility)
        {
            return UseComboAbility(comboAbility, direction, offsetDistance);
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

    public List<UsableAbilityInfo> GetUsableAbilities()
    {
        List<UsableAbilityInfo> usableAbilities = new();
        for(int i = 0; i < abilities.Count; i++)
        {
            Ability ability = abilities[i];
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

    private Ability GetAbility(int abilityNumber)
    {
        Ability ability = null;
        if (abilityNumber < abilities.Count)
        {
            ability = abilities[abilityNumber];
        }
        return ability;
    }

    private bool UseOnUseAbility(OnUseAbility onUseAbility, Vector2 direction, float offsetDistance)
    {
        if (entityState.CanAct() || CanCancelInto(onUseAbility))
        {
            EffectData abilityUse = StartCastingAbility(onUseAbility, direction);
            coroutine = DelayOnUseAbility(onUseAbility, abilityUse, offsetDistance);
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

    private bool UseComboAbility(ComboAbility comboAbility, Vector2 direction, float offsetDistance)
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
            EffectData abilityUse = StartCastingAbility(nextComboStage.Ability, direction);
            coroutine = DelayComboAbility(comboAbility, nextComboStage, abilityUse, offsetDistance);
            StartCoroutine(coroutine);
            return true;
        } else
        {
            return false;
        }
    }

    private EffectData StartCastingAbility(OnUseAbility onUseAbility, Vector2 direction)
    {
        EffectData abilityUse = new()
        {
            Position = transform.position,
            Entity = gameObject,
            EntityData = entityData,
            EntityState = entityState,
            Movement = movement,
            Damageable = damageable,
            Hitbox = hitbox,
            AbilityManager = this
        };

        if (movement != null)
        {
            movement.StopMoving();
        }

        entityState.LookDirection = direction;
        entityState.AbilityState(onUseAbility.RecoveryTime + onUseAbility.CastTime + onUseAbility.ActiveTime, onUseAbility.AimWhileCasting);
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
    /// <param name="offsetDistance"></param>
    /// <returns>IEnumerator used for the coroutine</returns>
    private IEnumerator DelayOnUseAbility(OnUseAbility onUseAbility, EffectData abilityUse, float offsetDistance)
    {
        yield return new WaitForSeconds(onUseAbility.CastTime);
        UpdateAbilityState(abilityUse, offsetDistance);
        onUseAbility.Use(abilityUse);
    }

    private IEnumerator DelayComboAbility(ComboAbility comboAbility, ComboStage nextComboStage, EffectData abilityUse, float offsetDistance)
    {
        yield return new WaitForSeconds(nextComboStage.Ability.CastTime);
        UpdateAbilityState(abilityUse, offsetDistance);
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

    private void UpdateAbilityState(EffectData abilityUse, float offsetDistance)
    {
        entityState.CanLookWhileCasting = false;
        abilityUse.Direction = entityState.LookDirection.normalized;
        abilityUse.Position += entityState.LookDirection.normalized * offsetDistance;
    }
}