using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component for managing hitbox collision events.
/// </summary>
public class Hitbox : MonoBehaviour
{
    private const float timeBetweenHits = 1;

    private Rigidbody2D body;
    private Dictionary<int, float> hitTimeByInstanceID = new();

    private EntityState entityState;
    private EntityData entityData;

    public event Action<EntityCollisionEvent> OnEntityCollision;

    private void Awake()
    {
        body = GetComponentInParent<Rigidbody2D>();
        entityState = GetComponentInParent<EntityState>();
        entityData = GetComponentInParent<EntityData>();
    }

    private void Update()
    {
        if (body != null && OnEntityCollision != null && OnEntityCollision.GetInvocationList().Length > 0)
        {
            body.sleepMode = RigidbodySleepMode2D.NeverSleep;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Hitbox") && IsHitTimerExceeded(collision))
        {
            EntityCollisionEvent entityCollisionEvent = new();
            entityCollisionEvent.SourceBody = body;
            entityCollisionEvent.TargetBody = collision.attachedRigidbody;
            entityCollisionEvent.SourceEntityData = entityData;
            entityCollisionEvent.SourceEntityState = entityState;
            OnEntityCollision?.Invoke(entityCollisionEvent);
        }
    }

    public void ResetHitTimer()
    {
        hitTimeByInstanceID = new();
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
        if (hitTimeByInstanceID.TryGetValue(otherInstanceID, out float lastHitTime))
        {
            if (currentTime - lastHitTime >= timeBetweenHits)
            {
                isHitTimerExceeded = true;
            }
        }
        else
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
