using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class wrapping a list of objects.
/// </summary>
public class SavedObjects
{
    public List<ObjectSave> ObjectList { get; set; } = new();
}
