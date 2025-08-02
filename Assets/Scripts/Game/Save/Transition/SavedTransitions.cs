using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class wrapping a list of transitions between areas.
/// </summary>
public class SavedTransitions
{
    public List<TransitionSave> TransitionList { get; set; } = new();
}
