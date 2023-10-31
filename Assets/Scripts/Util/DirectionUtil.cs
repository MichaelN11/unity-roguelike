using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Utility class for handling directions and 4-directional facing.
/// </summary>
public class DirectionUtil
{
    public static bool IsFacingUp(Vector2 direction)
    {
        Vector2 normalized = direction.normalized;
        return Mathf.Abs(normalized.y) > Mathf.Abs(normalized.x) && normalized.y > 0;
    }

    public static bool IsFacingDown(Vector2 direction)
    {
        Vector2 normalized = direction.normalized;
        return Mathf.Abs(normalized.y) >= Mathf.Abs(normalized.x) && normalized.y <= 0;
    }

    public static bool IsFacingRight(Vector2 direction)
    {
        Vector2 normalized = direction.normalized;
        return Mathf.Abs(normalized.x) > Mathf.Abs(normalized.y) && normalized.x > 0;
    }

    public static bool IsFacingLeft(Vector2 direction)
    {
        Vector2 normalized = direction.normalized;
        return Mathf.Abs(normalized.x) > Mathf.Abs(normalized.y) && normalized.x < 0;
    }
}
