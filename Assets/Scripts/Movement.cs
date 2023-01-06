using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component responsible for an entity's movement.
/// </summary>
public class Movement : MonoBehaviour
{
    public Vector2 Direction { get; set; }

    [SerializeField]
    private float speed;
    private Rigidbody2D body;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Move();
    }

    /// <summary>
    /// Sets the entity's velocity in the current direction using the movement speed.
    /// </summary>
    private void Move()
    {
        if (Direction != null)
        {
            Vector2 velocity = Direction.normalized;
            velocity.x *= speed;
            velocity.y *= speed;
            body.velocity = velocity;
        }
    }
}
