using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton class for playing sound effects.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private Dictionary<string, AudioSource> soundMap = new();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Plays the passed sound effect. Adds a new AudioSource component to the game object,
    /// if one hasn't already been added for the sound.
    /// </summary>
    /// <param name="sound">The Sound scriptable object</param>
    public void Play(Sound sound)
    {
        if (sound != null)
        {
            if (soundMap.TryGetValue(sound.name, out AudioSource audioSource))
            {
                Debug.Log("Found audio source for " + sound.name);
            }
            else
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.clip = sound.AudioClip;
                audioSource.volume = sound.Volume;
                audioSource.pitch = sound.Pitch;
                soundMap.Add(sound.name, audioSource);
                Debug.Log("Didn't find audio source, creating new one for: " + sound.name);
            }
            audioSource.Play();
        }
    }
}
