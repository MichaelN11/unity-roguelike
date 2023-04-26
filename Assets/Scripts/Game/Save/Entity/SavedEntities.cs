using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class wrapping a list of saved entities.
/// </summary>
[Serializable]
public class SavedEntities
{
    public List<EntitySave> EntityList { get; set; } = new();
}
