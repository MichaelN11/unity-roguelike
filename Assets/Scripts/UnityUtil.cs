using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
