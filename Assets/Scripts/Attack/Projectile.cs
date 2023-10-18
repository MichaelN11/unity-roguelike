using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component representing a projectile that moves in a straight path.
/// </summary>
public class Projectile : MonoBehaviour
{
    private const float ActiveTimeAfterWallHit = 0.1667f;

    public float Speed { get; set; }
    public Vector2 Direction { get; set; }
    public float WallStickDuration { get; set; } = 0;
    public float GroundStickDuration { get; set; } = 0;
    public float MaxDistance { get; set; } = 1;

    private Rigidbody2D body;
    private DamageObject damageObject;
    private Collider2D colliderComponent;
    private DestroyTimer destroyTimer;
    private float totalDistance = 0;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        damageObject = GetComponent<DamageObject>();
        colliderComponent = GetComponent<Collider2D>();
        destroyTimer = GetComponent<DestroyTimer>();
    }

    private void Start()
    {
        if (damageObject != null && damageObject.AttackData != null)
        {
            damageObject.AttackData.AttackEvents.OnAttackSuccessful += AttackSuccessful;
        }
    }

    private void FixedUpdate()
    {
        if (totalDistance > MaxDistance)
        {
            Stop();
            Destroy(gameObject, GroundStickDuration);
        }
        if (Direction != null && Direction != Vector2.zero && Speed > 0)
        {
            float distance = Speed * Time.deltaTime;
            totalDistance += distance;
            body.MovePosition(body.position + (distance * Direction.normalized));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (LayerUtil.IsWall(collision.gameObject.layer))
        {
            Stop();
            Invoke(nameof(DisableCollider), ActiveTimeAfterWallHit);
            Destroy(gameObject, WallStickDuration);
        }
    }

    /// <summary>
    /// Method called on a successful attack event for the projectile object.
    /// </summary>
    /// <param name="attackData">The AttackData</param>
    private void AttackSuccessful(AttackData attackData)
    {
        Stop();
        Destroy(gameObject, attackData.AttackEffectData.HitStop);
    }

    /// <summary>
    /// Stops the projectile and disables the collider component.
    /// </summary>
    private void Stop()
    {
        if (destroyTimer != null)
        {
            destroyTimer.enabled = false;
        }   
        Speed = 0;
    }

    private void DisableCollider()
    {
        if (colliderComponent != null)
        {
            colliderComponent.enabled = false;
        }
    }
}
