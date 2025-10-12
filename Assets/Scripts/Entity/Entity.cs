using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable object representing an entity type.
/// </summary>
[CreateAssetMenu(menuName = "Game Data/Entity")]
public class Entity : ScriptableObject
{
    [SerializeField]
    private GameObject baseObject;
    public GameObject BaseObject => baseObject;

    [SerializeField]
    private List<ActiveAbility> abilities;
    public List<ActiveAbility> Abilities => abilities;

    [SerializeField]
    private List<InventoryItem> initialInventory;
    public List<InventoryItem> InitialInventory => initialInventory;

    [SerializeField]
    private CharacterClass characterClass;
    public CharacterClass CharacterClass => characterClass;

    [SerializeField]
    private Sound soundOnHit;
    public Sound SoundOnHit => soundOnHit;

    [SerializeField]
    private Sound soundOnDeath;
    public Sound SoundOnDeath => soundOnDeath;

    [SerializeField]
    private Sound soundOnAggro;
    public Sound SoundOnAggro => soundOnAggro;

    [SerializeField]
    private float walkSpeed = 1f;
    public float WalkSpeed => walkSpeed;

    [SerializeField]
    private Vector2 interactionOffset = new Vector2(0.5f, 0.5f);
    public Vector2 InteractionOffset => interactionOffset;

    [SerializeField]
    private float maxHealth = 1;
    public float MaxHealth => maxHealth;

    [SerializeField]
    private float hitStunDuration = 1;
    public float HitStunDuration => hitStunDuration;

    [SerializeField]
    private float knockbackSpeed = 1;
    public float KnockbackSpeed => knockbackSpeed;

    [SerializeField]
    private float knockbackAcceleration = 0;
    public float KnockbackAcceleration => knockbackAcceleration;

    [SerializeField]
    private float flashOnHitTime = 0.15f;
    public float FlashOnHitTime => flashOnHitTime;

    [SerializeField]
    private float deathTimer;
    public float DeathTimer => deathTimer;

    [SerializeField]
    private EntityAI entityAI;
    public EntityAI EntityAI => entityAI;

    [SerializeField]
    private Weapon weapon;
    public Weapon Weapon => weapon;

    [SerializeField]
    private List<ItemDrop> itemDrops;
    public List<ItemDrop> ItemDrops => itemDrops;

    [SerializeField]
    private EntityDescription description;
    public EntityDescription Description => description;

    [SerializeField]
    private bool isBoss;
    public bool IsBoss => isBoss;

    [SerializeField]
    private List<Sound> footstepSounds;
    public List<Sound> FootstepSounds => footstepSounds;

    [SerializeField]
    private float footstepSoundInterval = 0.333f;
    public float FootstepSoundInterval => footstepSoundInterval;

    // TODO Remove this once everything is updated.
    [SerializeField]
    private bool rightLeftRework = false;
    public bool RightLeftRework => rightLeftRework;
}
