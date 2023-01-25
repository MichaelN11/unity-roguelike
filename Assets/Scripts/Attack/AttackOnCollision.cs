using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component for an object that Attacks on collision with a hitbox. Should have its AttackData
/// set in order to work.
/// </summary>
public class AttackOnCollision : MonoBehaviour
{
    public AttackData attackData;

    [SerializeField]
    private float timeBetweenHits = 1;

    private Rigidbody2D body;
    private Dictionary<int, float> hitTimeByInstanceID = new();

    private void Awake()
    {
        body = GetComponentInParent<Rigidbody2D>();
    }

    private void Start()
    {
        if (attackData.User == null)
        {
            attackData.User = UnityUtil.GetParentIfExists(gameObject);
        }
        if (body != null)
        {
            body.sleepMode = RigidbodySleepMode2D.NeverSleep;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Hitbox") && IsHitTimerExceeded(collision))
        {
            AttackEntity(collision);
        }
    }

    /// <summary>
    /// Finds the entity from the collision and passes it the attack data for the attack.
    /// </summary>
    /// <param name="collision">The Collider2D object</param>
    private void AttackEntity(Collider2D collision)
    {
        if (attackData.SetDirectionOnHit)
        {
            attackData.Direction = GetAttackDirection(collision);
        }
        
        EntityController otherEntityController = collision.gameObject.GetComponentInParent<EntityController>();
        if (otherEntityController != null
            && IsValidAttackTarget(collision.gameObject, otherEntityController.EntityType))
        {
            otherEntityController.HandleIncomingAttack(attackData);
            AudioManager.Instance.Play(attackData.AttackType.SoundOnHit);
        }
    }

    /// <summary>
    /// Gets the attack direction from the collision, using the position of the rigidbodies.
    /// </summary>
    /// <param name="collision">The Collider2D object</param>
    /// <returns>The attack direction</returns>
    private Vector2 GetAttackDirection(Collider2D collision)
    {
        Vector2 direction = Vector2.zero;
        if (body != null)
        {
            Vector2 otherPosition = collision.attachedRigidbody.position;
            Vector2 thisPosition = body.position;
            direction = otherPosition - thisPosition;
        }
        return direction;
    }

    /// <summary>
    /// Determines if the passed collision object's hit timer is exceeded, by checking if time
    /// passed from the last time the object was hit is greater than the time between hits.
    /// If it is attackable, the hit time for the object is updated.
    /// </summary>
    /// <param name="collision">The Collider2D object</param>
    /// <returns>true if the object is attackable</returns>
    private bool IsHitTimerExceeded(Collider2D collision)
    {
        bool isHitTimerExceeded = false;

        int otherInstanceID = collision.gameObject.GetInstanceID();
        float currentTime = Time.fixedTime;
        if (hitTimeByInstanceID.TryGetValue(otherInstanceID, out float lastHitTime)) {
            if (currentTime - lastHitTime >= timeBetweenHits)
            {
                isHitTimerExceeded = true;
            }
        } else
        {
            isHitTimerExceeded = true;
        }

        if (isHitTimerExceeded)
        {
            hitTimeByInstanceID[otherInstanceID] = currentTime;
        }

        return isHitTimerExceeded;
    }

    /// <summary>
    /// Determines if the passed entity is a valid attack target for the attack.
    /// </summary>
    /// <param name="entity">The entity GameObject</param>
    /// <param name="entityType">The EntityType containing data about the entity</param>
    /// <returns>true if the entity is a valid target for the attack</returns>
    private bool IsValidAttackTarget(GameObject entity, EntityType entityType)
    {
        return entity != attackData.User
            && attackData.EntityType.EnemyFactions.Contains(entityType.Faction);
    }
}
