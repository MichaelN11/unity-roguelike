using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component responsible for an entity's movement.
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Movement : MonoBehaviour
{
    private const float CollisionOffset = 0.05f;

    public Vector2 Direction { get; private set; } = Vector2.zero;
    public float Speed { get; private set; } = 0;
    public float Acceleration { get; private set; } = 0;

    private ContactFilter2D contactFilter2D = new();
    private EntityState entityState;
    private Rigidbody2D body;
    private Collider2D movementCollider;
    private List<RaycastHit2D> raycastHits = new();
    private List<Collider2D> colliderHits = new();

    private void Awake()
    {
        entityState = GetComponent<EntityState>();
        body = GetComponent<Rigidbody2D>();
        movementCollider = GetComponent<Collider2D>();
    }

    private void FixedUpdate()
    {
        if (entityState == null || !entityState.IsStopped())
        {
            Vector2 movePosition = CalculateMoveOutOfCollisions();
            if (Direction != null
                && Direction != Vector2.zero)
            {
                UpdateSpeed();
                if (movePosition == body.position)
                {
                    movePosition = CalculateMove();
                }
            }
            if (movePosition != body.position)
            {
                body.MovePosition(movePosition);
            }
        }
    }

    /// <summary>
    /// Sets the movement speed and direction.
    /// </summary>
    /// <param name="direction">The Vector2 direction to move in</param>
    /// <param name="speed">The movement speed</param>
    public void SetMovement(Vector2 direction, float speed)
    {
        SetMovement(direction, speed, 0);
    }

    /// <summary>
    /// Sets the movement speed, direction, and acceleration.
    /// </summary>
    /// <param name="direction">The Vector2 direction to move in</param>
    /// <param name="speed">The movement speed</param>
    /// <param name="acceleration">The movement acceleration</param>
    public void SetMovement(Vector2 direction, float speed, float acceleration)
    {
        Direction = direction;
        Speed = speed;
        Acceleration = acceleration;
    }

    /// <summary>
    /// Stops the entity from moving.
    /// </summary>
    public void StopMoving()
    {
        SetMovement(Vector2.zero, 0);
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
    /// Calculates a move out of any collisions overlapping the object. If there are
    /// multiple objects overlapping, the move speed is reduced and the final move
    /// is added together. This prevents the object from getting stuck on other objects.
    /// </summary>
    /// <returns>A Vector2 position moving out of collisions, or body.position if there are none</returns>
    private Vector2 CalculateMoveOutOfCollisions()
    {
        Vector2 movePosition = body.position;
        int numOverlaps = movementCollider.OverlapCollider(new(), colliderHits);
        foreach (Collider2D collider in colliderHits)
        {
            Vector2 collisionPoint = collider.ClosestPoint(body.position);
            float distance = (Speed / numOverlaps) * Time.deltaTime;
            Vector2 moveDirection = (body.position - collisionPoint).normalized;
            movePosition += moveDirection * distance;
        }
        return movePosition;
    }

    /// <summary>
    /// Calculates the next move using the movement direction. If the movement is blocked by a collision,
    /// it attempts to slide along the collision in the x or y direction.
    /// </summary>
    /// <returns>A Vector2 position for the next move, or body.position if the move was blocked</returns>
    private Vector2 CalculateMove()
    {
        //Debug.Log("Attempting normal move " + gameObject.name);
        Vector2 normalizedDirection = Direction.normalized;
        Vector2 movePosition = CalculateMoveInDirection(normalizedDirection);
        if (movePosition == body.position)
        {
            movePosition = CalculateMoveInDirection(new Vector2(normalizedDirection.x, 0));
            if (movePosition == body.position)
            {
                movePosition = CalculateMoveInDirection(new Vector2(0, normalizedDirection.y));
            }
        }
        return movePosition;
    }

    /// <summary>
    /// Calculates the next move in the passed direction using the movement speed, if there is no collision
    /// in the way.
    /// </summary>
    /// <returns>A Vector2 position for the next move, or body.position if the move was blocked</returns>
    private Vector2 CalculateMoveInDirection(Vector2 direction)
    {
        Vector2 movePosition = body.position;
        float distance = Speed * Time.deltaTime;
        float offsetDistance = DetermineOffsetDistance(direction);
        int collisionCount = movementCollider.Cast(direction,
            contactFilter2D,
            raycastHits,
            distance + offsetDistance);
        if (collisionCount == 0)
        {
            movePosition = body.position + direction * distance;
        } else
        {
            movePosition = CalculateMoveToNearestCollision(direction, distance, offsetDistance);
        }
        return movePosition;
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
        offsetDirection.x += CollisionOffset * xSign;
        offsetDirection.y += CollisionOffset * ySign;
        return offsetDirection.magnitude - direction.magnitude;
    }

    /// <summary>
    /// Calculates a move up to the nearest collision in the collisions list.
    /// </summary>
    /// <param name="direction">The direction being moved in</param>
    /// <param name="initialDistance">The initial distance of the collision check</param>
    /// <param name="offsetDistance">The offset distance used for collision offset</param>
    /// <returns>The Vector2 move position, or body.position if the movement was blocked</returns>
    private Vector2 CalculateMoveToNearestCollision(Vector2 direction, float initialDistance, float offsetDistance)
    {
        Vector2 movePosition = body.position;
        float shortestDistance = initialDistance + offsetDistance;
        foreach (RaycastHit2D collision in raycastHits)
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
            movePosition = body.position + direction * distanceToCollision;
        }
        return movePosition;
    }
}
