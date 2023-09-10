using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// POCO for storing data for the instance of an Attack.
/// </summary>
[Serializable]
public class AttackData
{
    [SerializeField]
    private bool setDirectionOnHit = true;
    public bool SetDirectionOnHit
    {
        get => setDirectionOnHit;
        set { setDirectionOnHit = value; }
    }

    public AttackEffectData EffectData { get; set; }
    public GameObject User { get; set; }
    public EntityState UserEntityState { get; set; }
    public Vector2 Direction { get; set; } = Vector2.zero;
    public EntityData UserEntityData { get; set; }
    public AttackEvents AttackEvents { get; set; } = new AttackEvents();
}
