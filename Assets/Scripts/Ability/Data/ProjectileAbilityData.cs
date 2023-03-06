using System;
using UnityEngine;

/// <summary>
/// POCO containing data for a projectile used by an ability. Used with the PrefabAbilityData.
/// </summary>
[Serializable]
public class ProjectileAbilityData
{
    [SerializeField]
    private PrefabAbilityData prefabAbilityData;
    public PrefabAbilityData PrefabAbilityData => prefabAbilityData;

    [SerializeField]
    private float speed = 0;
    public float Speed => speed;

    [SerializeField]
    private float wallStickDuration = 0;
    public float WallStickDuration => wallStickDuration;
}
