using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// POCO holding entity AI data.
/// </summary>
[Serializable]
public class EntityAI
{
    [SerializeField]
    private float aggroDistance = 5;
    public float AggroDistance => aggroDistance;
}
