using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component for an object that damages on collision with a hitbox. Should have its AttackData
/// set in order to work.
/// </summary>
public class DamageObject : MonoBehaviour
{
    public AttackData AttackData { get; set; }

    private Rigidbody2D body;
    private Hitbox hitbox;

    private void Awake()
    {
        body = GetComponentInParent<Rigidbody2D>();
        hitbox = GetComponent<Hitbox>();
    }

    private void Start()
    {
        if (AttackData.User == null)
        {
            AttackData.User = UnityUtil.GetParentIfExists(gameObject);
        }
        if (hitbox != null)
        {
            hitbox.OnEntityCollision += AttackOnCollision;
        }
    }

    private void AttackOnCollision(EntityCollisionEvent entityCollisionEvent)
    {
        AttackHandler.AttackEntity(AttackData, body, entityCollisionEvent.TargetBody);
    }
}
