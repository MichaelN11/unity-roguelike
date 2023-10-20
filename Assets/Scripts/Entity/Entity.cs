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
    private List<Ability> abilities;
    public List<Ability> Abilities => abilities;

    [SerializeField]
    private Sound soundOnHit;
    public Sound SoundOnHit => soundOnHit;

    [SerializeField]
    private float walkSpeed = 1f;
    public float WalkSpeed => walkSpeed;

    [SerializeField]
    private float interactionDistance = 0.5f;
    public float InteractionDistance => interactionDistance;

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
    private float dropChance = 0;
    public float DropChance => dropChance;

    [SerializeField]
    private GameObject droppable;
    public GameObject Droppable => droppable;

    [SerializeField]
    private EntityDescription description;
    public EntityDescription Description => description;
}
