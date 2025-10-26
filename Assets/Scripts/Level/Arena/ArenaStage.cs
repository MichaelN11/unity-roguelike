using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A stage in an arena, containing either a static wave or a difficulty.
/// </summary>
[Serializable]
public class ArenaStage
{
    [field: SerializeField]
    public WaveDifficulty Difficulty { get; private set; }

    [field: SerializeField]
    public ArenaWave StaticWave { get; private set; }
}
