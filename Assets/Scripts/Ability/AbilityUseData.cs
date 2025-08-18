using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class storing data for an ability being triggered, that is used by the ability's effects.
/// </summary>
public class AbilityUseData
{
    public GameObject Entity { get; set; }
    public EntityData EntityData { get; set; }
    public EntityState EntityState { get; set; }
    public Movement Movement { get; set; }
    public Damageable Damageable { get; set; }
    public Hitbox Hitbox { get; set; }
    public AbilityManager AbilityManager { get; set; }
    public SpriteRenderer SpriteRenderer { get; set; }
    public List<EffectUseData> EffectUseDataList { get; set; } = new();

    public Vector2 Position { get; set; }
    public Vector2 Direction { get; set; } = Vector2.zero;

    public float ChargePercent { get; set; } = 0;
}
