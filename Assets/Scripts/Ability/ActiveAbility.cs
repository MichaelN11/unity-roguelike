using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract class marking an active ability scriptable object.
/// </summary>
public abstract class ActiveAbility : ScriptableObject
{
    [SerializeField]
    private float cooldown;
    public float Cooldown => cooldown;
}
