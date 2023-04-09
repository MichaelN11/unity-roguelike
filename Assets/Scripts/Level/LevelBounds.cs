using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component for determining the boundaries of a level.
/// </summary>
public class LevelBounds : MonoBehaviour
{
    private Bounds bounds;
    public Bounds Bounds => bounds;

    private void Awake()
    {
        bounds = new Bounds();
        bounds.SetMinMax(transform.position, transform.position + new Vector3(transform.localScale.x, transform.localScale.y));
    }
}
