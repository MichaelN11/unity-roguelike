using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackBehavior : AbilityBehavior
{
    public override float Range => rangedAttack.AttackAbilityData.Range;

    protected override float CastTime => rangedAttack.CastTime;

    private readonly GameObject user;
    private readonly Movement movement;
    private readonly EntityState entityState;
    private readonly EntityData entityData;
    private readonly AnimatorUpdater animatorUpdater;

    private RangedAttack rangedAttack;

    public RangedAttackBehavior(RangedAttack rangedAttack, AbilityManager abilityManager)
    {
        this.rangedAttack = rangedAttack;
        user = UnityUtil.GetParentIfExists(abilityManager.gameObject);
        movement = user.GetComponent<Movement>();
        entityState = user.GetComponent<EntityState>();
        entityData = user.GetComponent<EntityData>();
        animatorUpdater = user.GetComponent<AnimatorUpdater>();
    }

    public override bool IsUsable(AbilityUse abilityUse)
    {
        return entityState.CanAct();
    }

    protected override AbilityUseEventInfo OnUse(AbilityUse abilityUse)
    {
        if (movement != null)
        {
            movement.StopMoving();
        }
        if (animatorUpdater != null)
        {
            animatorUpdater.LookDirection = abilityUse.Direction;
        }
        entityState.AbilityState(rangedAttack.AttackAbilityData.RecoveryDuration + CastTime);

        return new AbilityUseEventInfo()
        {
            AbilityUse = abilityUse,
            AbilityAnimation = rangedAttack.AttackAnimation
        };
    }

    protected override void StartAbility(AbilityUse abilityUse)
    {
        AttackData attackData = AttackAbilityUtil.BuildAttackData(abilityUse, rangedAttack.AttackAbilityData, entityData);
        attackData.AttackEvents.OnAttackSuccessful += AttackSuccessful;

        GameObject instance = AttackAbilityUtil.InstantiateDamageObject(abilityUse,
            rangedAttack.AttackAbilityData,
            rangedAttack.ProjectileAbilityData.PrefabAbilityData,
            attackData);

        Projectile projectile = instance.GetComponent<Projectile>();
        projectile.Speed = rangedAttack.ProjectileAbilityData.Speed;
        projectile.Direction = abilityUse.Direction;
    }

    /// <summary>
    /// Called after a successful attack.
    /// </summary>
    /// <param name="attackData">The attack data from the successful attack</param>
    private void AttackSuccessful(AttackData attackData)
    {
        AudioManager.Instance.Play(attackData.AbilityData.SoundOnHit);
    }
}
