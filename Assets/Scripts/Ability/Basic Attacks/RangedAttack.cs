using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ability scriptable object representing a ranged attack.
/// </summary>
[CreateAssetMenu(menuName = "Game Data/Ability/Ranged Attack")]
public class RangedAttack : Ability
{
    [SerializeField]
    private AbilityAnimation attackAnimation = AbilityAnimation.Default;
    public AbilityAnimation AttackAnimation => attackAnimation;

    [SerializeField]
    private AttackAbilityData attackAbilityData;
    public AttackAbilityData AttackAbilityData => attackAbilityData;

    [SerializeField]
    private ProjectileAbilityData projectileAbilityData;
    public ProjectileAbilityData ProjectileAbilityData => projectileAbilityData;

    [SerializeField]
    private float castTime = 0f;
    public float CastTime => castTime;

    public override AbilityBehavior BuildBehavior(AbilityManager abilityManager)
    {
        return new RangedAttackBehavior(this, abilityManager);
    }
}
