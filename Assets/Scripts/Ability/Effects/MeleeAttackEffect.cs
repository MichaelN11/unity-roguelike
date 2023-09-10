using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An AbilityEffect that does a melee attack.
/// </summary>
[CreateAssetMenu(menuName = "Game Data/Ability Effect/Melee Attack")]
public class MeleeAttackEffect : AbilityEffect
{
    [SerializeField]
    private PrefabEffectData prefabEffectData;
    public PrefabEffectData PrefabEffectData => prefabEffectData;

    [SerializeField]
    private AttackEffectData attackEffectData;
    public AttackEffectData AttackEffectData => attackEffectData;

    public override void Trigger(EffectData effectData)
    {
        AttackData attackData = AttackEffectUtil.BuildAttackData(effectData, attackEffectData);
        attackData.AttackEvents.OnAttackSuccessful += AttackSuccessful;

        GameObject instance = AttackEffectUtil.InstantiateDamageObject(effectData,
            attackEffectData,
            prefabEffectData,
            attackData);

        instance.transform.parent = effectData.Entity.transform;
    }

    /// <summary>
    /// Called after a successful attack.
    /// </summary>
    /// <param name="attackData">The attack data from the successful attack</param>
    private void AttackSuccessful(AttackData attackData)
    {
        AudioManager.Instance.Play(attackEffectData.SoundOnHit);
        attackData.UserEntityState.Stop(attackData.EffectData.HitStop);
    }
}
