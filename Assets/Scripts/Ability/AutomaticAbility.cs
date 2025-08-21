using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A stateless, automatic ability that is activated regardless of the entity's current state/position.
/// </summary>
[CreateAssetMenu(menuName = "Game Data/Ability/Automatic")]
public class AutomaticAbility : ActiveAbility
{
    [SerializeField]
    private List<AbilityEffect> effects;
    public List<AbilityEffect> Effects => effects;

    [SerializeField]
    private Sound soundOnUse;
    public Sound SoundOnUse => soundOnUse;

    [SerializeField]
    private float duration;
    public float Duration => duration;

    [SerializeField]
    private float range;
    public float Range => range;

    [SerializeField]
    private AbilityAnimation abilityAnimation;
    public AbilityAnimation AbilityAnimation => abilityAnimation;

    public override AbilityUseEventInfo Use(Vector2 direction, float offsetDistance, AbilityUseData abilityUse, EntityAbilityContext entityAbilityContext)
    {
        AbilityUseEventInfo abilityUseEvent = BuildAbilityUseEventInfo(abilityUse);
        abilityUse.AbilityManager.InvokeAbilityStartedEvent(abilityUseEvent);
        abilityUse.AbilityManager.InvokeAbilityUseEvent(abilityUseEvent);
        abilityUse.Direction = direction;
        abilityUse.Position += direction * offsetDistance;
        Activate(abilityUse);

        return abilityUseEvent;
    }

    private void Activate(AbilityUseData abilityUse)
    {
        if (soundOnUse != null)
        {
            AudioManager.Instance.Play(soundOnUse);
        }
        AbilityUtil.ActivateEffects(effects, abilityUse, duration);
    }

    private AbilityUseEventInfo BuildAbilityUseEventInfo(AbilityUseData abilityUse)
    {
        AbilityUseEventInfo abilityUseEvent = new()
        {
            AbilityUse = abilityUse,
            AbilityAnimation = AbilityAnimation,
            Range = Range,
            StatelessCast = true
        };
        return abilityUseEvent;
    }
}
