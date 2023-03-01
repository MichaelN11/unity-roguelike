using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component representing a projectile that moves in a straight path.
/// </summary>
public class Projectile : MonoBehaviour
{
    public float Speed { get; set; }
    public Vector2 Direction { get; set; }

    private Rigidbody2D body;
    private DamageObject damageObject;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        damageObject = GetComponent<DamageObject>();
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
        if (Direction != null && Direction != Vector2.zero && Speed > 0)
        {
            body.MovePosition(body.position + (Speed * Time.deltaTime * Direction.normalized));
        }
    }

    /// <summary>
    /// Method called on a successful attack event for the projectile object.
    /// </summary>
    /// <param name="attackData">The AttackData</param>
    private void AttackSuccessful(AttackData attackData)
    {
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }
        Speed = 0;
        Destroy(gameObject, attackData.AbilityData.HitStop);
    }
}
