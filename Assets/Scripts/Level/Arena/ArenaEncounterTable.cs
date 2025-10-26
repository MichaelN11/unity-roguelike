using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject for storing arena encounters by difficulty for a level.
/// </summary>
[CreateAssetMenu(menuName = "Game Data/Arena Encounter Table")]
public class ArenaEncounterTable : ScriptableObject
{
    [field: SerializeField]
    public List<ArenaWave> VeryEasyWaves { get; private set; } = new List<ArenaWave>();

    [field: SerializeField]
    public List<ArenaWave> EasyWaves { get; private set; } = new List<ArenaWave>();

    [field: SerializeField]
    public List<ArenaWave> MediumWaves { get; private set; } = new List<ArenaWave>();

    [field: SerializeField]
    public List<ArenaWave> HardWaves { get; private set; } = new List<ArenaWave>();

    [field: SerializeField]
    public List<ArenaWave> VeryHardWaves { get; private set; } = new List<ArenaWave>();

    /// <summary>
    /// Workaround for Unity not initializing serialized lists with default values.
    /// https://issuetracker.unity3d.com/issues/serializefield-list-objects-are-not-initialized-with-class-slash-struct-default-values-when-adding-objects-in-the-inspector-window
    /// </summary>
    private void OnValidate()
    {
        if (VeryEasyWaves == null || VeryEasyWaves.Count == 0)
        {
            VeryEasyWaves = new List<ArenaWave> { new ArenaWave() };
        }
        if (EasyWaves == null || EasyWaves.Count == 0)
        {
            EasyWaves = new List<ArenaWave> { new ArenaWave() };
        }
        if (MediumWaves == null || MediumWaves.Count == 0)
        {
            MediumWaves = new List<ArenaWave> { new ArenaWave() };
        }
        if (HardWaves == null || HardWaves.Count == 0)
        {
            HardWaves = new List<ArenaWave> { new ArenaWave() };
        }
        if (VeryHardWaves == null || VeryHardWaves.Count == 0)
        {
            VeryHardWaves = new List<ArenaWave> { new ArenaWave() };
        }
    }
}
