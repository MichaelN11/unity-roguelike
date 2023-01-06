using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enum representing a direction for 8-directional movement.
/// </summary>
[Obsolete("Use an x and a y value instead for directional facing")]
public enum Direction
{
    None,
    Up,
    Down,
    Left,
    Right,
    UpLeft,
    UpRight,
    DownLeft,
    DownRight
}

/// <summary>
/// Helper class for methods involving the Direction enum.
/// </summary>
[Obsolete("Use an x and a y value instead for directional facing")]
public static class DirectionMethods
{
    /// <summary>
    /// Determines the direction from the passed Vector2. Returns None for the zero vector.
    /// </summary>
    /// <param name="directionVector">A Vector2 representing a direction</param>
    /// <returns>The Direction corresponding to the passed Vector2, None if the vector is zero</returns>
    public static Direction DetermineDirection(Vector2 directionVector)
    {
        Direction direction = Direction.None;
        if (directionVector.y > 0)
        {
            if (directionVector.x > 0)
            {
                direction = Direction.UpRight;
            }
            else if (directionVector.x < 0)
            {
                direction = Direction.UpLeft;
            }
            else
            {
                direction = Direction.Up;
            }
        }
        else if (directionVector.y < 0)
        {
            if (directionVector.x > 0)
            {
                direction = Direction.DownRight;
            }
            else if (directionVector.x < 0)
            {
                direction = Direction.DownLeft;
            }
            else
            {
                direction = Direction.Down;
            }
        }
        else if (directionVector.x < 0)
        {
            direction = Direction.Left;
        }
        else if (directionVector.x > 0)
        {
            direction = Direction.Right;
        }

        return direction;
    }
}
