using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component for an object that Attacks on collision with a hitbox. Should have its AttackData
/// set in order to work.
/// </summary>
public class AttackOnHit : MonoBehaviour
{
    public AttackData attackData;

    [SerializeField]
    private float timeBetweenHits;

    private Rigidbody2D body;
    private Dictionary<int, float> hitTimeByInstanceID = new();

    private void Start()
    {
        if (attackData.user == null)
        {
            attackData.user = gameObject;
        }
        if (attackData.setDirectionOnHit)
        {
            body = GetComponentInParent<Rigidbody2D>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsValidAttackTarget(collision) && IsHitTimerExceeded(collision))
        {
            AttackEntity(collision);
        }
    }

    /// <summary>
    /// Determines if the passed collision is a valid attack target's hitbox.
    /// </summary>
    /// <param name="collision">The Collider2D object</param>
    /// <returns>true if the target is valid for the attack</returns>
    private bool IsValidAttackTarget(Collider2D collision)
    {
        return collision.gameObject.CompareTag("Hitbox")
            && collision.gameObject.transform.parent.gameObject != attackData.user;
    }

    /// <summary>
    /// Finds the entity from the collision and passes it the attack data for the attack.
    /// </summary>
    /// <param name="collision">The Collider2D object</param>
    private void AttackEntity(Collider2D collision)
    {
        if (attackData.setDirectionOnHit)
        {
            attackData.direction = GetAttackDirection(collision);
        }
        
        EntityController otherEntityController = collision.gameObject.GetComponentInParent<EntityController>();
        if (otherEntityController != null)
        {
            otherEntityController.HandleIncomingAttack(attackData);
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
}
