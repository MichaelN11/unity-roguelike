using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An ability that has a combo of different abilities that can be used in succession.
/// </summary>
[CreateAssetMenu(menuName = "Game Data/Ability/Combo")]
public class ComboAbility : ActiveAbility
{
    [SerializeField]
    private List<ComboStage> comboStages;
    public List<ComboStage> ComboStages => comboStages;

    public override AbilityUseEventInfo Use(Vector2 direction, float offsetDistance,
        AbilityUseData abilityUse, EntityAbilityContext entityAbilityContext)
    {
        if (entityAbilityContext.CurrentComboAbility != this)
        {
            ResetCombo(entityAbilityContext);
            entityAbilityContext.CurrentComboAbility = this;
        }
        if (abilityUse.EntityState.CanAct()
                || (abilityUse.EntityState.ActionState == ActionState.Hardcasting
                && abilityUse.EntityState.StunTimer <= entityAbilityContext.ComboableTime))
        {
            entityAbilityContext.ComboTimer = 0;
            ComboStage nextComboStage = ComboStages[entityAbilityContext.NextComboNumber];
            AbilityUseEventInfo abilityUseEvent = nextComboStage.Ability.StartCastingAbility(direction, abilityUse, entityAbilityContext);
            entityAbilityContext.DelayedAbilityCoroutine = DelayComboAbility(nextComboStage, abilityUseEvent.AbilityUse, offsetDistance, entityAbilityContext);
            abilityUse.AbilityManager.StartCoroutine(entityAbilityContext.DelayedAbilityCoroutine);
            return abilityUseEvent;
        }
        else
        {
            return null;
        }
    }

    public static void UpdateComboState(EntityAbilityContext entityAbilityContext)
    {
        if (entityAbilityContext.ComboTimer > 0)
        {
            entityAbilityContext.ComboTimer -= Time.deltaTime;
            if (entityAbilityContext.ComboTimer <= 0)
            {
                ResetCombo(entityAbilityContext);
            }
        }
    }

    public static void ResetCombo(EntityAbilityContext entityAbilityContext)
    {
        entityAbilityContext.NextComboNumber = 0;
        entityAbilityContext.ComboTimer = 0;
        entityAbilityContext.ComboableTime = 0;
    }

    private IEnumerator DelayComboAbility(ComboStage nextComboStage, AbilityUseData abilityUse,
        float offsetDistance,EntityAbilityContext entityAbilityContext)
    {
        yield return new WaitForSeconds(nextComboStage.Ability.CastTime);
        AbilityUtil.UpdateAbilityState(abilityUse, offsetDistance, entityAbilityContext);
        nextComboStage.Ability.Activate(abilityUse);

        AbilityUseEventInfo abilityUseEvent = nextComboStage.Ability.BuildAbilityUseEventInfo(abilityUse);
        abilityUseEvent.Origin = entityAbilityContext.CurrentAbilityOrigin;
        abilityUse.AbilityManager.InvokeAbilityUseEvent(abilityUseEvent);

        if (entityAbilityContext.NextComboNumber + 1 < ComboStages.Count)
        {
            entityAbilityContext.ComboTimer = nextComboStage.ComboContinueWindow;
            entityAbilityContext.ComboableTime = nextComboStage.ComboCancelableDuration;
            ++entityAbilityContext.NextComboNumber;
        }
        else
        {
            ResetCombo(entityAbilityContext);
        }
    }
}
