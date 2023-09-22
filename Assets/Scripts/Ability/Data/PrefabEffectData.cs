using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// POCO containing prefab data for an effect.
/// </summary>
[Serializable]
public class PrefabEffectData
{
    [SerializeField]
    private GameObject prefab;
    public GameObject Prefab => prefab;

    [SerializeField]
    private float prefabDuration = 1;
    public float PrefabDuration => prefabDuration;

    [SerializeField]
    private bool rotatePrefab = true;
    public bool RotatePrefab => rotatePrefab;

    /// <summary>
    /// If the prefab should be mirrored in the x direction instead of rotated 180.
    /// </summary>
    [SerializeField]
    private bool mirrorPrefabX = false;
    public bool MirrorPrefabX => mirrorPrefabX;
}
