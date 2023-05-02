using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton class for playing sound effects.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public float MusicVolume { get; set; } = 1;
    public float SfxVolume { get; set; } = 1;

    private readonly Dictionary<string, AudioSource> soundMap = new();
    private AudioSource currentMusic;

    private void Awake()
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
            if (!soundMap.TryGetValue(sound.name, out AudioSource audioSource))
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.clip = sound.AudioClip;
                audioSource.volume = sound.Volume;
                audioSource.pitch = sound.Pitch;
                audioSource.loop = sound.Loop;
                soundMap.Add(sound.name, audioSource);
            }

            if (audioSource == currentMusic && audioSource.isPlaying)
            {
                return;
            }

            float volume;
            if (sound.IsMusic)
            {
                StopMusic();
                volume = MusicVolume;
                currentMusic = audioSource;
            } else
            {
                volume = SfxVolume;
            }
            audioSource.PlayOneShot(sound.AudioClip, volume);
        }
    }

    /// <summary>
    /// Stops the current music.
    /// </summary>
    public void StopMusic()
    {
        if (currentMusic != null)
        {
            currentMusic.Stop();
        }
    }
}
