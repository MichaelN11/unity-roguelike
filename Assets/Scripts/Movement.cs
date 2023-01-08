using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component responsible for an entity's movement.
/// </summary>
public class Movement : MonoBehaviour
{
    public Vector2 Direction { get; set; }
    public float Speed { get; set; } = 0;
    public float Acceleration { get; set; } = 0;

    [SerializeField]
    private float collisionOffset = 0.05f;
    [SerializeField]
    private ContactFilter2D contactFilter2D;

    private Rigidbody2D body;
    private Collider2D movementCollider;
    private List<RaycastHit2D> collisions = new();

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        movementCollider = GetComponent<Collider2D>();
    }

    private void FixedUpdate()
    {
        if (Direction != null && Direction != Vector2.zero)
        {
            UpdateSpeed();
            AttemptMovement();
        }
    }

    /// <summary>
    /// Updates the movement speed and direction.
    /// </summary>
    /// <param name="direction">The Vector2 direction to move in</param>
    /// <param name="speed">The movement speed</param>
    public void UpdateMovement(Vector2 direction, float speed)
    {
        UpdateMovement(direction, speed, 0);
    }

    /// <summary>
    /// Updates the movement speed, direction, and acceleration.
    /// </summary>
    /// <param name="direction">The Vector2 direction to move in</param>
    /// <param name="speed">The movement speed</param>
    /// <param name="acceleration">The movement acceleration</param>
    public void UpdateMovement(Vector2 direction, float speed, float acceleration)
    {
        Direction = direction;
        Speed = speed;
        Acceleration = acceleration;
    }

    /// <summary>
    /// Updates the Speed with the Acceleration. If Speed falls below 0, both the
    /// Speed and Acceleration are set to 0.
    /// </summary>
    private void UpdateSpeed()
    {
        Speed += Acceleration;
        if (Speed <= 0)
        {
            Speed = 0;
            Acceleration = 0;
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
        float distance = Speed * Time.deltaTime;
        float offsetDistance = DetermineOffsetDistance(direction);
        int collisionCount = movementCollider.Cast(direction,
            contactFilter2D,
            collisions,
            distance + offsetDistance);
        if (collisionCount == 0)
        {
            Move(body.position, direction * distance);
            moved = true;
        } else
        {
            moved = MoveToNearestCollision(direction, distance, offsetDistance);
        }
        return moved;
    }

    /// <summary>
    /// Determines the collision offset distance using the passed direction Vector2.
    /// When checking collision, the offset is added to the distance, but we want to
    /// unnormalize it so that the offset is a constant x or y offset from a collider.
    /// </summary>
    /// <param name="direction">The direction being moved in</param>
    /// <returns>The offset distance needed for the offset in the x or y direction</returns>
    private float DetermineOffsetDistance(Vector2 direction)
    {
        Vector2 offsetDirection = direction;
        float xSign = (offsetDirection.x != 0) ? Mathf.Sign(offsetDirection.x) : 0;
        float ySign = (offsetDirection.y != 0) ? Mathf.Sign(offsetDirection.y) : 0;
        offsetDirection.x += collisionOffset * xSign;
        offsetDirection.y += collisionOffset * ySign;
        return offsetDirection.magnitude - direction.magnitude;
    }

    /// <summary>
    /// Moves up to the nearest collision in the collisions list. Returns true if
    /// the move was successful.
    /// </summary>
    /// <param name="direction">The direction being moved in</param>
    /// <param name="initialDistance">The initial distance of the collision check</param>
    /// <param name="offsetDistance">The offset distance used for collision offset</param>
    /// <returns></returns>
    private bool MoveToNearestCollision(Vector2 direction, float initialDistance, float offsetDistance)
    {
        bool moved = false;
        float shortestDistance = initialDistance + offsetDistance;
        foreach (RaycastHit2D collision in collisions)
        {
            float distanceToCollider = collision.distance;
            if (distanceToCollider < shortestDistance)
            {
                shortestDistance = distanceToCollider;
            }
        }
        float distanceToCollision = shortestDistance - offsetDistance;
        if (distanceToCollision > 0.0001)
        {
            Move(body.position, direction * distanceToCollision);
            moved = true;
        }
        return moved;
    }

    /// <summary>
    /// Moves from the old location, to the new location, using the Rigidbody's MovePosition.
    /// Clamps both locations to a pixel perfect position.
    /// </summary>
    /// <param name="oldLocation">The old location</param>
    /// <param name="newLocation">The new location being moved to</param>
    private void Move(Vector2 oldLocation, Vector2 newLocation)
    {
        body.MovePosition(oldLocation + newLocation);
        //body.MovePosition(PixelPerfectClamp(oldLocation, pixelsPerUnit) + PixelPerfectClamp(newLocation, pixelsPerUnit));
    }

    /// <summary>
    /// Clamps the passed movement vector to a rounded pixel position, in order to
    /// reduce jittering.
    /// </summary>
    /// <param name="moveVector">The Vector2 representing a position used for movement</param>
    /// <param name="pixelsPerUnit">The number of pixels per unit for the sprite</param>
    /// <returns>Precise pixel location based on pixels per unit</returns>
    [Obsolete("Doesn't seem necessary with pixel perfect camera")]
    private Vector2 PixelPerfectClamp(Vector2 moveVector, float pixelsPerUnit)
    {
        Vector2 vectorInPixels = new Vector2(
            Mathf.RoundToInt(moveVector.x * pixelsPerUnit),
            Mathf.RoundToInt(moveVector.y * pixelsPerUnit));
        return vectorInPixels / pixelsPerUnit;
    }
}
