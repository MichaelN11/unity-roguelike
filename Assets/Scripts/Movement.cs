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
    private float speed = 1;
    [SerializeField]
    private float collisionOffset = 0.05f;
    [SerializeField]
    private ContactFilter2D contactFilter2D;

    private Rigidbody2D body;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Debug.Log("Moving in direction: " + Direction + " Delta time: " + Time.deltaTime);
        if (Direction != Vector2.zero)
        {
            AttemptMovement();
        }
    }

    /// <summary>
    /// Attempts to move using the movement direction. If the movement is blocked by a collision,
    /// it attempts to slide along the collision in the x or y direction.
    /// </summary>
    /// <returns>true if movement was successful</returns>
    private bool AttemptMovement()
    {
        Vector2 normalizedDirection = Direction.normalized;
        bool moved = MoveIfNoCollision(normalizedDirection);
        if (!moved)
        {
            moved = MoveIfNoCollision(new Vector2(normalizedDirection.x, 0));
            if (!moved)
            {
                moved = MoveIfNoCollision(new Vector2(0, normalizedDirection.y));
            }
        }
        return moved;
    }

    /// <summary>
    /// Moves in the passed direction using the movement speed, if there is no collision
    /// in the way.
    /// </summary>
    /// <returns>true if the movement was not blocked by a collision</returns>
    private bool MoveIfNoCollision(Vector2 direction)
    {
        bool moved = false;
        float distance = speed * Time.deltaTime;
        int collisionCount = body.Cast(direction,
            contactFilter2D,
            new List<RaycastHit2D>(),
            distance + collisionOffset);
        if (collisionCount == 0)
        {
            body.MovePosition(body.position + direction * distance);
            moved = true;
        }
        return moved;
    }
}
