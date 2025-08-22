using System;
using UnityEngine;

/// <summary>
/// POCO containing data for a projectile used by an effect.
/// </summary>
[Serializable]
public class ProjectileEffectData
{
    [SerializeField]
    private PrefabEffectData prefabEffectData;
    public PrefabEffectData PrefabEffectData => prefabEffectData;

    [SerializeField]
    private float range = 0;
    public float Range => range;

    [SerializeField]
    private float speed = 0;
    public float Speed => speed;

    [SerializeField]
    private float wallStickDuration = 0;
    public float WallStickDuration => wallStickDuration;

    [SerializeField]
    private float groundStickDuration = 0;
    public float GroundStickDuration => groundStickDuration;

    [SerializeField]
    private float rangeIncreaseFromCharge = 0;
    public float RangeIncreaseFromCharge => rangeIncreaseFromCharge;

    [SerializeField]
    private float speedIncreaseFromCharge = 0;
    public float SpeedIncreaseFromCharge => speedIncreaseFromCharge;
}
