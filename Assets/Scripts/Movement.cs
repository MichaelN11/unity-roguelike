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

    [SerializeField]
    private float collisionOffset = 0.05f;
    [SerializeField]
    private ContactFilter2D contactFilter2D;

    private Rigidbody2D body;

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (Direction != null && Direction != Vector2.zero)
        {
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
        Direction = direction;
        Speed = speed;
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
        int collisionCount = body.Cast(direction,
            contactFilter2D,
            new List<RaycastHit2D>(),
            distance + collisionOffset);
        if (collisionCount == 0)
        {
            Move(body.position, direction * distance);
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
