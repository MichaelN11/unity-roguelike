using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// POCO representing a transition between areas in the level.
/// </summary>
[Serializable]
public class TransitionSave
{
    public float Rotation { get; set; }
    public Vector2 Position { get; set; }
    public bool IsStart { get; set; }
    public bool IsEnd { get; set; }
    public bool IsWinCondition { get; set; }
    public bool IsVisible { get; set; }
    public string NewScene { get; set; }
    public string TransitionName { get; set; }
}
