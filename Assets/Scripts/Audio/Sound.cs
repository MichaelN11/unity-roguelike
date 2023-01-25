using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable object representing a playable sound effect.
/// </summary>
[CreateAssetMenu(menuName = "Game Data/Sound")]
public class Sound : ScriptableObject
{
    [SerializeField]
    private AudioClip audioClip;
    public AudioClip AudioClip => audioClip;

    [SerializeField]
    [Range(0f, 1f)]
    private float volume = 1f;
    public float Volume => volume;

    [SerializeField]
    [Range(0.1f, 3f)]
    private float pitch = 1f;
    public float Pitch => pitch;
}
