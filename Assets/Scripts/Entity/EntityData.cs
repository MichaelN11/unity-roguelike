using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component for storing entity data.
/// </summary>
public class EntityData : MonoBehaviour
{
    [SerializeField]
    private EntityType entityType;
    public EntityType EntityType => entityType;
}
