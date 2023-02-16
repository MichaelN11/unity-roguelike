using UnityEngine;

/// <summary>
/// Scriptable object representing an attack type.
/// </summary>
[CreateAssetMenu(menuName = "Game Data/Attack Type")]
public class AttackType : ScriptableObject
{
    [SerializeField]
    private GameObject prefab;
    public GameObject Prefab => prefab;

    [SerializeField]
    private Sound soundOnUse;
    public Sound SoundOnUse => soundOnUse;

    [SerializeField]
    private Sound soundOnHit;
    public Sound SoundOnHit => soundOnHit;

    [SerializeField]
    private AttackAnimation attackAnimation = AttackAnimation.Default;
    public AttackAnimation AttackAnimation => attackAnimation;

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

    [SerializeField]
    private float attackDuration = 1f;
    public float AttackDuration => attackDuration;

    [SerializeField]
    private float comboableAttackDuration = 0f;
    public float ComboableAttackDuration => comboableAttackDuration;

    [SerializeField]
    private float comboContinueWindow = 1f;
    public float ComboContinueWindow => comboContinueWindow;

    [SerializeField]
    private float moveSpeed = 0;
    public float MoveSpeed => moveSpeed;

    [SerializeField]
    private float moveAcceleration = 0;
    public float MoveAcceleration => moveAcceleration;

    [SerializeField]
    private float hitStop = 0.06f;
    public float HitStop => hitStop;

    [SerializeField]
    private bool rotateAttackObject = true;
    public bool RotateAttackObject => rotateAttackObject;
}
