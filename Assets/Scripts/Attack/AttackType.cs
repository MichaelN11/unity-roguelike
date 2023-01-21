using UnityEngine;

/// <summary>
/// Scriptable object representing an attack type.
/// </summary>
[CreateAssetMenu(menuName = "GameData/AttackType")]
public class AttackType : ScriptableObject
{
    [SerializeField]
    private Animation animation;
    public Animation Animation => animation;

    [SerializeField]
    private float damage = 1;
    public float Damage => damage;

    [SerializeField]
    private float range = 0;
    public float Range => range;

    [SerializeField]
    private float hitboxDuration = 1;
    public float HitboxDuration => hitboxDuration;

    [SerializeField]
    private float hitStunMultiplier = 1;
    public float HitStunMultiplier => hitStunMultiplier;

    [SerializeField]
    private float knockbackMultiplier = 1;
    public float KnockbackMultiplier => knockbackMultiplier;

    [SerializeField]
    private float startupTime = 0;
    public float StartupTime => startupTime;
}
