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
    /// The event that fires when an ability cast is started by the entity.
    /// </summary>
    public event Action<AbilityUseEventInfo> OnAbilityStarted;
    /// <summary>
    /// The event that fires when an ability is done casting and is used by the entity.
    /// </summary>
    public event Action<AbilityUseEventInfo> OnAbilityUse;

    private List<ActiveAbilityContext> abilities;
    public List<ActiveAbilityContext> Abilities => abilities;

    private EntityData entityData;
    private EntityState entityState;
    private Movement movement;
    private Damageable damageable;
    private Hitbox hitbox;
    private SpriteRenderer spriteRenderer;

    private readonly EntityAbilityContext entityAbilityContext = new();
    public EntityAbilityContext EntityAbilityContext => entityAbilityContext;

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
        ComboAbility.UpdateComboState(entityAbilityContext);

        if (entityAbilityContext.CurrentActiveAbility != null && entityAbilityContext.CurrentAbilityStarted)
        {
            entityAbilityContext.CurrentAbilityDuration += Time.deltaTime;
        }

        foreach (ActiveAbilityContext ability in abilities)
        {
            if (ability.CurrentCooldown > 0)
            {
                ability.CurrentCooldown -= Time.deltaTime;
            }
        }

        if (entityAbilityContext.CurrentActiveAbility)
        {
            entityAbilityContext.CurrentActiveAbility.UpdateAbility(entityAbilityContext);
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
            List<ActiveAbilityContext> abilityContexts = new();
            foreach (ActiveAbility ability in abilities)
            {
                abilityContexts.Add(new()
                {
                    Ability = ability
                });
            }
            abilityManager.abilities = abilityContexts;
        }
        return abilityManager;
    }

    public int LearnNewAbility(ActiveAbility ability)
    {
        int abilityNumber = -1;

        bool isNewAbility = true;
        for (int i = 0; i < abilities.Count; i++) 
        {
            ActiveAbilityContext abilityContext = abilities[i];
            if (abilityContext.Ability == ability)
            {
                isNewAbility = false;
                break;
            } else if (ability.AbilityUniqueType != AbilityUniqueType.None
                && abilityContext.Ability.AbilityUniqueType == ability.AbilityUniqueType)
            {
                abilities[i] = new()
                {
                    Ability = ability
                };
                abilityNumber = i;
                isNewAbility = false;
                break;
            }
        }
        if (isNewAbility)
        {
            abilities.Add(new()
            {
                Ability = ability
            });
            abilityNumber = abilities.Count - 1;
        }

        return abilityNumber;
    }

    /// <summary>
    /// Uses the ability.
    /// </summary>
    /// <param name="abilityNumber">The number of the used ability in the hotbar/UI</param>
    /// <param name="direction">The direction in which the ability was used</param>
    /// <param name="offset">The distance offset from the entity</param>
    /// <returns>AbilityUseEventInfo, or null if unsuccessful</returns>
    public AbilityUseEventInfo UseAbility(int abilityNumber, Vector2 direction, Vector2 offset)
    {
        ActiveAbilityContext ability = GetAbility(abilityNumber);
        if (ability == null)
        {
            return null;
        }

        return UseAbility(ability, direction, offset);
    }

    public AbilityUseEventInfo UseAbility(ActiveAbilityContext ability, Vector2 direction, Vector2 offset, string origin = null)
    {
        AbilityUseEventInfo abilityUseEventInfo = null;

        if (ability.CurrentCooldown <= 0)
        {
            AbilityUseData abilityUse = BuildAbilityUseData(ability, offset);
            if (!ability.Ability.CanActivate(abilityUse, entityAbilityContext))
            {
                return null;
            }
            else
            {
                abilityUseEventInfo = ability.Ability.Use(direction, abilityUse, entityAbilityContext);
            }
        }

        // Ability cast was successful
        if (abilityUseEventInfo != null)
        {
            entityAbilityContext.CurrentAbilityOrigin = origin;
        }

        return abilityUseEventInfo;
    }

    /// <summary>
    /// A held ability is released. Used for chargeable abilities.
    /// </summary>
    /// <param name="abilityNumber">The number of the used ability in the hotbar/UI</param>
    /// <param name="direction">The direction in which the ability was used</param>
    /// <param name="offset">The distance offset from the entity</param>
    /// <returns>true if the ability was stopped by the release</returns>
    public bool ReleaseAbility(int abilityNumber, Vector2 direction, Vector2 offset)
    {
        ActiveAbilityContext ability = GetAbility(abilityNumber);
        if (ability != null)
        {
            AbilityUseData abilityUse = BuildAbilityUseData(ability, offset);
            return ability.Ability.Release(direction, abilityUse, entityAbilityContext);
        } else
        {
            return false;
        }
    }

    /// <summary>
    /// Interrupts all coroutines and the current ability.
    /// </summary>
    public void InterruptAll()
    {
        ComboAbility.ResetCombo(entityAbilityContext);
        StopAllCoroutines();
        InterruptCurrentAbility();
    }

    public List<UsableAbilityInfo> GetUsableAbilities()
    {
        AbilityUseData abilityUse = BuildAbilityUseData(null, entityData.Entity.InteractionOffset);
        List<UsableAbilityInfo> usableAbilities = new();
        for(int i = 0; i < abilities.Count; i++)
        {
            ActiveAbilityContext abilityContext = abilities[i];
            abilityUse.AbilityContext = abilityContext;
            UsableAbilityInfo usableAbilityInfo = abilityContext.Ability.GetUsableAbilityInfo(abilityUse, entityAbilityContext);
            usableAbilityInfo.AbilityNumber = i;
            usableAbilities.Add(usableAbilityInfo);
        }
        return usableAbilities;
    }

    public void InvokeAbilityStartedEvent(AbilityUseEventInfo abilityUseEvent)
    {
        OnAbilityStarted?.Invoke(abilityUseEvent);
    }

    public void InvokeAbilityUseEvent(AbilityUseEventInfo abilityUseEvent)
    {
        ActiveAbilityContext context = abilityUseEvent.AbilityUse.AbilityContext;
        context.CurrentCooldown = context.Ability.Cooldown;
        OnAbilityUse?.Invoke(abilityUseEvent);
    }

    private ActiveAbilityContext GetAbility(int abilityNumber)
    {
        ActiveAbilityContext ability = null;
        if (abilityNumber < abilities.Count)
        {
            ability = abilities[abilityNumber];
        }
        return ability;
    }

    private AbilityUseData BuildAbilityUseData(ActiveAbilityContext abilityContext, Vector2 offset)
    {
        return new()
        {
            AbilityContext = abilityContext,
            Position = transform.position,
            Entity = gameObject,
            EntityData = entityData,
            EntityState = entityState,
            Movement = movement,
            Damageable = damageable,
            Hitbox = hitbox,
            SpriteRenderer = spriteRenderer,
            Offset = offset,
            AbilityManager = this
        };
    }

    public void InterruptCurrentAbility()
    {
        if (entityAbilityContext.DelayedAbilityCoroutine != null)
        {
            StopCoroutine(entityAbilityContext.DelayedAbilityCoroutine);
        }

        if (entityAbilityContext.CurrentActiveAbility != null && entityAbilityContext.CurrentAbilityData != null)
        {
            entityAbilityContext.CurrentActiveAbility.Interrupt(entityAbilityContext.CurrentAbilityData,
                entityAbilityContext.CurrentAbilityDuration, entityAbilityContext);

            if (entityAbilityContext.CurrentAbilityStarted)
            {
                entityAbilityContext.CurrentAbilityStarted = false;
            }
            entityAbilityContext.CurrentActiveAbility = null;
            entityAbilityContext.CurrentAbilityOrigin = null;
        }
    }
}