using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable object representing an entity type.
/// </summary>
[CreateAssetMenu(menuName = "Game Data/Entity Type")]
public class EntityType : ScriptableObject
{
    [SerializeField]
    private float walkSpeed = 1f;
    public float WalkSpeed => walkSpeed;
    [SerializeField]
    private float interactionDistance = 0.5f;
    public float InteractionDistance => interactionDistance;
    [SerializeField]
    private float attackDuration = 1f;
    public float AttackDuration => attackDuration;
    [SerializeField]
    private Faction faction;
    public Faction Faction => faction;
    [SerializeField]
    private List<Faction> enemyFactions;
    public List<Faction> EnemyFactions => enemyFactions;
}
