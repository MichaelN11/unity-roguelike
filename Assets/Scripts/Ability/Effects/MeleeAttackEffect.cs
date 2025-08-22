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

    public override void Trigger(AbilityUseData abilityUseData, EffectUseData effectUseData)
    {
        AttackData attackData = AttackEffectUtil.BuildAttackData(abilityUseData, attackEffectData);
        attackData.AttackEvents.OnAttackSuccessful += AttackSuccessful;

        GameObject instance = AttackEffectUtil.InstantiateDamageObject(abilityUseData,
            attackEffectData,
            prefabEffectData,
            attackData);
        effectUseData.CreatedObjects.Add(instance);

        instance.transform.parent = abilityUseData.Entity.transform;
    }

    public override void Unapply(AbilityUseData abilityUseData, EffectUseData effectUseData)
    {
        if (effectUseData.CreatedObjects.Count > 0)
        {
            Destroy(effectUseData.CreatedObjects[0]);
        }
    }

    /// <summary>
    /// Use the duration of the created attack object, if it is greater than the effect duration.
    /// </summary>
    public override float Duration
    {
        get { return (base.Duration >= prefabEffectData.PrefabDuration) ? base.Duration : prefabEffectData.PrefabDuration; }
    }

    /// <summary>
    /// Called after a successful attack.
    /// </summary>
    /// <param name="attackData">The attack data from the successful attack</param>
    private void AttackSuccessful(AttackData attackData)
    {
        AudioManager.Instance.Play(attackEffectData.SoundOnHit);
        attackData.UserEntityState.Stop(attackData.HitStop);
    }
}
