using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component for managing an entity's abilities.
/// </summary>
public class AbilityManager : MonoBehaviour
{
    public event Action UpdateEvent;

    [SerializeField]
    private Ability ability;

    private AbilityBehavior abilityBehavior;

    private void Awake()
    {
        abilityBehavior = ability.BuildBehavior(this);
    }

    private void Update()
    {
        UpdateEvent?.Invoke();
    }

    /// <summary>
    /// Uses the ability
    /// </summary>
    /// <param name="abilityUse">Object containing ability use data</param>
    /// <returns>true if the ability was used successfully</returns>
    public bool UseAbility(AbilityUse abilityUse)
    {
        abilityUse.Component = this;
        return abilityBehavior.Use(abilityUse);
    }

    /// <summary>
    /// Interrupts the ability.
    /// </summary>
    public void Interrupt()
    {
        abilityBehavior.Interrupt(this);
    }

    /// <summary>
    /// Determines if the ability is usable.
    /// </summary>
    /// <param name="abilityUse">Object containing ability use data</param>
    /// <returns>true if the ability is usable</returns>
    public bool IsUsable(AbilityUse abilityUse)
    {
        abilityUse.Component = this;
        return abilityBehavior.IsUsable(abilityUse);
    }

    /// <summary>
    /// Gets the ability range for the AI. Will need to be updated in the future to return more data.
    /// </summary>
    /// <returns>The ability range as a float</returns>
    public float GetRange()
    {
        return abilityBehavior.Range;
    }
}
