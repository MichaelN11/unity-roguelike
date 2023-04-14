using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// POCO representing a saved entity.
/// </summary>
public class EntitySave
{
    public string Name { get; set; }
    public Vector2 Position { get; set; }
    public float Health { get; set; }
}
