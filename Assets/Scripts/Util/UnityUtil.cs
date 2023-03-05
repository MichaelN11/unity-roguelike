using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Utility class for static methods.
/// </summary>
public class UnityUtil
{
    /// <returns>The parent game object if one exists, otherwise returns the passed game object.</returns>
    public static GameObject GetParentIfExists(GameObject gameObject)
    {
        GameObject user = gameObject;
        Transform parentTransform = gameObject.transform.parent;
        if (parentTransform != null)
        {
            user = parentTransform.gameObject;
        }
        return user;
    }

    /// <summary>
    /// Determines the a Quaternion rotation to rotate an object facing right to the
    /// passed Vector2 direction.
    /// </summary>
    /// <param name="direction">The direction as a Vector2</param>
    /// <returns>The rotation as a Quaternion</returns>
    public static Quaternion RotateTowardsVector(Vector2 direction)
    {
        float angle = Vector2.SignedAngle(Vector2.right, direction);
        return Quaternion.Euler(0, 0, angle);
    }

    /// <summary>
    /// Clamps the passed movement vector to a rounded pixel position, in order to
    /// reduce jittering.
    /// </summary>
    /// <param name="moveVector">The Vector2 representing a position used for movement</param>
    /// <param name="pixelsPerUnit">The number of pixels per unit for the sprite</param>
    /// <returns>Precise pixel location based on pixels per unit</returns>
    public static Vector2 PixelPerfectClamp(Vector2 moveVector, float pixelsPerUnit)
    {
        Vector2 vectorInPixels = new Vector2(
            Mathf.RoundToInt(moveVector.x * pixelsPerUnit),
            Mathf.RoundToInt(moveVector.y * pixelsPerUnit));
        return vectorInPixels / pixelsPerUnit;
    }
}
