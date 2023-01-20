using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Component used for using an attack.
/// </summary>
public class Attack : MonoBehaviour
{
    public AttackStats attackStats;

    [SerializeField]
    private string attackObjectResourceName = "AttackObject";

    private readonly List<Faction> allFactions = Enum.GetValues(typeof(Faction)).OfType<Faction>().ToList();

    private GameObject attackPrefab;
    private Vector2 direction;
    private float distance;
    private bool interrupted = false;
    private List<Faction> targetFactions;

    private void Awake()
    {
        attackPrefab = (GameObject) Resources.Load(attackObjectResourceName);
    }

    /// <summary>
    /// Use the attack. Waits for the startup time before starting the attack.
    /// </summary>
    /// <param name="direction">The direction of the attack</param>
    /// <param name="distance">The distance away the attack is used</param>
    public void Use(Vector2 direction, float distance)
    {
        Use(direction, distance, allFactions);
    }

    /// <summary>
    /// Use the attack. Waits for the startup time before starting the attack.
    /// </summary>
    /// <param name="direction">The direction of the attack</param>
    /// <param name="distance">The distance away the attack is used</param>
    /// <param name="targetFactions">The factions targeted by the attack</param>
    public void Use(Vector2 direction, float distance, List<Faction> targetFactions)
    {
        interrupted = false;
        this.direction = direction;
        this.distance = distance;
        this.targetFactions = targetFactions;
        Invoke(nameof(StartAttack), attackStats.startupTime);
    }

    /// <summary>
    /// Interrupts the attack, preventing it from starting if it hasn't already started.
    /// </summary>
    public void Interrupt()
    {
        interrupted = true;
    }

    /// <summary>
    /// Starts the attack. Attacks in the passed direction, at the passed distance.
    /// An AttackDamage object is created with the AttackData set.
    /// </summary>
    private void StartAttack()
    {
        if (!interrupted)
        {
            float angle = Vector2.SignedAngle(Vector2.right, direction) - 90f;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            distance += attackStats.range;
            Vector3 position = transform.position + (Vector3)direction.normalized * distance;
            GameObject instance = Instantiate(attackPrefab, position, rotation);

            DestroyTimer destroyTimer = instance.GetComponent<DestroyTimer>();
            destroyTimer.Duration = attackStats.hitboxDuration;

            AttackOnHit attackObject = instance.GetComponent<AttackOnHit>();
            AttackUseData attackData = new();
            attackData.attackStats = attackStats;
            attackData.User = UnityUtil.GetParentIfExists(gameObject);
            attackData.Direction = direction;
            attackData.setDirectionOnHit = false;
            attackData.targetFactions = targetFactions;
            attackObject.attackData = attackData;
        }
    }
}
