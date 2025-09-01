using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract class marking an active ability scriptable object.
/// </summary>
public abstract class ActiveAbility : ScriptableObject
{
    [SerializeField]
    private string abilityName = "";
    public string AbilityName => abilityName;

    [SerializeField]
    private Sprite abilityIcon;
    public Sprite AbilityIcon => abilityIcon;

    [SerializeField]
    private float cooldown;
    public float Cooldown => cooldown;

    [SerializeField]
    private AbilityUniqueType abilityUniqueType;
    public AbilityUniqueType AbilityUniqueType => abilityUniqueType;

    [SerializeField]
    private List<CharacterClass> allowedClasses;
    public List<CharacterClass> AllowedClasses => allowedClasses;

    [SerializeField]
    private List<AbilityCondition> abilityConditions = new();

    public abstract AbilityUseEventInfo Use(Vector2 direction, float offsetDistance, AbilityUseData abilityUse, EntityAbilityContext entityAbilityContext);

    public virtual bool CanActivate(AbilityUseData abilityUse, EntityAbilityContext entityAbilityContext)
    {
        foreach (AbilityCondition condition in abilityConditions)
        {
            if (condition && !condition.ConditionMet(abilityUse, entityAbilityContext))
            {
                return false;
            }
        }
        return true;
    }

    public virtual bool Release(Vector2 direction, float offsetDistance, AbilityUseData abilityUse, EntityAbilityContext entityAbilityContext)
    {
        return false;
    }

    public virtual void Interrupt(AbilityUseData abilityUse, float currentDuration, EntityAbilityContext entityAbilityContext) { }

    public virtual UsableAbilityInfo GetUsableAbilityInfo(AbilityUseData abilityUse, EntityAbilityContext entityAbilityContext)
    {
        return new UsableAbilityInfo();
    }

    public virtual void UpdateAbility(EntityAbilityContext entityAbilityContext) { }
}
