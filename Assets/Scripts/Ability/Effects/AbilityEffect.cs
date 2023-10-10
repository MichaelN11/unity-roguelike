using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract class representing an effect used by an ability.
/// </summary>
public abstract class AbilityEffect : ScriptableObject
{
    [SerializeField]
    private Sound soundOnUse;
    public Sound SoundOnUse => soundOnUse;

    [SerializeField]
    private float duration;
    public float Duration => duration;

    [SerializeField]
    private bool useAbilityDuration;
    public bool UseAbilityDuration => useAbilityDuration;

    public abstract void Trigger(EffectData effectData);

    public virtual void Unapply(EffectData effectData) { }
}
