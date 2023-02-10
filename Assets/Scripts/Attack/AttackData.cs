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
    private AttackType attackType;
    public AttackType AttackType
    {
        get => attackType;
        set { attackType = value; }
    }

    [SerializeField]
    private bool setDirectionOnHit = true;
    public bool SetDirectionOnHit
    {
        get => setDirectionOnHit;
        set { setDirectionOnHit = value; }
    }

    public GameObject User { get; set; }
    public Vector2 Direction { get; set; } = Vector2.zero;
    public EntityType EntityType { get; set; }
    public AttackEvents AttackEvents { get; set; }
}
