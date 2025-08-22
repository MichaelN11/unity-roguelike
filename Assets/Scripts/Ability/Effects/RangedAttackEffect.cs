using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An AbilityEffect that does a ranged attack. Supports charging.
/// </summary>
[CreateAssetMenu(menuName = "Game Data/Ability Effect/Ranged Attack")]
public class RangedAttackEffect : AbilityEffect
{
    [SerializeField]
    private ProjectileEffectData projectileEffectData;
    public ProjectileEffectData ProjectileEffectData => projectileEffectData;

    [SerializeField]
    private AttackEffectData attackEffectData;
    public AttackEffectData AttackEffectData => attackEffectData;

    public override void Trigger(AbilityUseData abilityUseData, EffectUseData effectUseData)
    {
        AttackData attackData = AttackEffectUtil.BuildAttackData(abilityUseData, attackEffectData);

        float range = projectileEffectData.Range;
        float speed = projectileEffectData.Speed;

        if (abilityUseData.ChargePercent > 0)
        {
            range += Mathf.Lerp(0, projectileEffectData.RangeIncreaseFromCharge, abilityUseData.ChargePercent);
            speed += Mathf.Lerp(0, projectileEffectData.SpeedIncreaseFromCharge, abilityUseData.ChargePercent);
            attackData.Damage += Mathf.Lerp(0, attackEffectData.DamageIncreaseFromCharge, abilityUseData.ChargePercent);
            attackData.HitStunMultiplier += Mathf.Lerp(0, attackEffectData.HitStunIncreaseFromCharge, abilityUseData.ChargePercent);
            attackData.KnockbackMultiplier += Mathf.Lerp(0, attackEffectData.KnockbackIncreaseFromCharge, abilityUseData.ChargePercent);
        }

        attackData.AttackEvents.OnAttackSuccessful += AttackSuccessful;

        GameObject instance = AttackEffectUtil.InstantiateDamageObject(abilityUseData,
            attackEffectData,
            projectileEffectData.PrefabEffectData,
            attackData);
        effectUseData.CreatedObjects.Add(instance);

        Projectile projectile = instance.GetComponent<Projectile>();
        projectile.Speed = speed;
        projectile.Direction = abilityUseData.Direction;
        projectile.MaxDistance = range;
        projectile.WallStickDuration = projectileEffectData.WallStickDuration;
        projectile.GroundStickDuration = projectileEffectData.GroundStickDuration;
    }

    /// <summary>
    /// Called after a successful attack.
    /// </summary>
    /// <param name="attackData">The attack data from the successful attack</param>
    private void AttackSuccessful(AttackData attackData)
    {
        AudioManager.Instance.Play(attackEffectData.SoundOnHit);
    }
}