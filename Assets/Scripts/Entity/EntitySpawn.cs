using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for defining an entity to spawn and how many of it to spawn.
/// </summary>
[Serializable]
public class EntitySpawn
{
    public Entity Entity;
    public int Amount = 1;
}
