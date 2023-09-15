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

    public abstract void Trigger(EffectData effectData);

    public virtual void Interrupt(EffectData effectData) { }
}
